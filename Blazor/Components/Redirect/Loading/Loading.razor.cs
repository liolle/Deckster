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
  private bool Resolved { get; set; } = false;

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

  private async void Init(int timeout)
  {
    if (Resolved || timeout > 5000)
    {
      StopLoading();
      StateHasChanged();
      return;
    }
    await Task.Delay(timeout);
    Init(timeout * 2);
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (!firstRender)
    { return; }
    Init(200);
    await UpdateGameState();
  }

  private async Task UpdateGameState()
  {
    if (_matchService is not null)
    {
      string state = await _matchService.GetPlayerState();
      HandleGameState(state);
      return;
    }
  }

  private void HandleGameState(string state)
  {

    switch (state)
    {
      case "Blazor.services.game.state.PlayerLobby":
        if (_matchService is not null)
        {
          _matchService.State = MatchState.lobby;
        }

        Navigate("/");
        break;
      case "Blazor.services.game.state.PlayerPlaying":
        if (_matchService is not null)
        {
          _matchService.State = MatchState.playing;
        }

        Navigate("/game");
        break;

      default:
        if (_matchService is not null)
        {
          _matchService.State = MatchState.lobby;
        }
        Navigate("/");
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
  }

  private void HandleJoinGame(GameMatch gameMatch, Player player)
  {
    if (_clockService is not null)
    {
      _clockService.Stop();
      _clockService.Reset();
      _clockService.ShowClock(false);
    }
    Navigate("/game");
  }

  private void OnGameLeft()
  {
    Navigate("/");
  }

  private void Navigate(string location)
  {
    Match matcher = Regex.Match(navigation?.Uri ?? "", @"(https|http):\/\/[a-zA-Z0-9.:]*\/([a-zA-Z0-9]*)[^\/]*$");
    string loc = matcher.Groups[2].Value;
    StopLoading();
    Resolved = true;
    if (location == $"/{loc}")
    {
      StateHasChanged();
      return;
    }

    navigation?.NavigateTo(location);
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
