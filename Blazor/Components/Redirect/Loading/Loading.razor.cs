using System.Text.RegularExpressions;
using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazor.Components.Redirect.Loading;

public partial class Loading : ComponentBase, IDisposable
{
  [Inject]
  private MatchService? _matchService { get; set; }

  [Inject]
  private NavigationManager? navigation { get; set; }

  [Inject]
  ClockService? _clockService { get; set; }

  private bool IsLoading { get; set; } = true;
  private bool Cancelled { get; set; } = false;

  public bool ValidateClaims(AuthenticationState context)
  {
    return context.User.HasClaim(value => value.Type == "Id");
  }

  protected override void OnInitialized()
  {
    if (_matchService is null)
    {
      return;
    }

    _matchService.JoinGame += HandleJoinGame;
    _matchService.OnGameLeft += OnGameLeft;

    if (navigation is not null)
    {
      navigation.LocationChanged += HandleLocationChange;

    }

    Init();
  }

  private async void HandleLocationChange(object? sender, LocationChangedEventArgs e)
  {
    if (_matchService is null)
    {
      return;
    }

    string state = await _matchService.GetPlayerState() ?? "";
    if (state.Split('.').Last() == "PlayerTempDisconnection")
    {
      _ = _matchService.SearchGameAsync();
    }
  }

  private async void Init()
  {
    await Task.Delay(1000);
    if (!Cancelled)
    {
      StopLoading();
    }
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (!firstRender)
    { return; }

    if (_matchService is not null)
    {
      string state = await _matchService.GetPlayerState();

      HandleGameState(state);
    }
  }


  private void HandleGameState(string state)
  {
    Match game_matcher = Regex.Match(navigation?.Uri ?? "", @"(https|http):\/\/[a-zA-Z0-9.:]*\/game[^\/]*$");

    switch (state)
    {
      case "Blazor.services.game.state.PlayerLobby":
        if (_matchService is not null)
        {
          _matchService.State = MatchState.lobby;
        }

        if (game_matcher.Success)
        {
          navigation?.NavigateTo("/");
        }
        break;
      case "Blazor.services.game.state.PlayerPlaying":
        if (_matchService is not null)
        {
          _matchService.State = MatchState.playing;
        }
        StopLoading();
        break;

      default:
        break;
    }
  }

  private void StopLoading()
  {
    IsLoading = false;
    if (_matchService is not null)
    {
      _matchService.IsLoading = false;
    }
    StateHasChanged();
  }

  private void HandleJoinGame(GameMatch gameMatch, Player player)
  {
    Cancelled = true;
    StopLoading();

    navigation?.NavigateTo("/game");
    if (_clockService is not null)
    {
      _clockService.Stop();
      _clockService.Reset();
      _clockService.ShowClock(false);
    }
  }

  private void OnGameLeft()
  {
    navigation?.NavigateTo("/");
  }

  public void Dispose()
  {
    if (_matchService is not null)
    {
      _matchService.JoinGame -= HandleJoinGame;
      _matchService.OnGameLeft -= OnGameLeft;
    }

    if (navigation is not null)
    {
      navigation.LocationChanged -= HandleLocationChange;
    }
  }
}
