using System.Text.RegularExpressions;
using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazor.Components.Redirect.Loading;

public partial class Loading : ComponentBase, IDisposable
{
  [Inject]
  private MatchService? MatchService { get; set; }

  [Inject]
  private NavigationManager? Navigation { get; set; }

  [Inject]
  ClockService? ClockService { get; set; }

  private bool IsLoading { get; set; } = true;
  private bool Resolved { get; set; } = false;

  public bool ValidateClaims(AuthenticationState context)
  {
    return context.User.HasClaim(value => value.Type == "Id");
  }

  protected override void OnInitialized()
  {
    if (MatchService is null)
    {
      return;
    }

    MatchService.JoinGame += HandleJoinGame;
    MatchService.OnGameLeft += OnGameLeft;

    if (Navigation is not null)
    {
      Navigation.LocationChanged += HandleLocationChange;

    }
  }

  private async void HandleLocationChange(object? sender, LocationChangedEventArgs e)
  {
    if (MatchService is null)
    {
      return;
    }

    string state = await MatchService.GetPlayerState() ?? "";
    if (state.Split('.').Last() == "PlayerTempDisconnection")
    {
      _ = MatchService.SearchGameAsync();
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
    if (MatchService is not null)
    {
      string state = await MatchService.GetPlayerState();
      HandleGameState(state);
      return;
    }
  }

  // may need to re-think this if more protected pages are needed
  private void HandleGameState(string state)
  {
    Match matcher = Regex.Match(Navigation?.Uri ?? "", @"(https|http):\/\/[a-zA-Z0-9.:]*\/([a-zA-Z0-9]*)[^\/]*$");
    string loc = matcher.Groups[2].Value;
    
    
    // page, player state, 
    switch (state)
    {
      case nameof(PlayerLobby):
        if (MatchService is not null)
        {
          MatchService.State = MatchState.lobby;
        }

        if (loc == "game")
        {
          Navigate("/");
        }
        else
        {
          Navigate(loc);
        }
        
        break;
      case nameof(PlayerInGame):
        if (MatchService is not null)
        {
          MatchService.State = MatchState.playing;
        }

        Navigate("/game");
        break;

      default:
        if (MatchService is not null)
        {
          MatchService.State = MatchState.lobby;
        }
        Navigate(loc);
        break;
    }
  }

  private void StopLoading()
  {
    IsLoading = false;
    if (MatchService is not null)
    {
      MatchService.IsLoading = false;
    }
  }

  private void HandleJoinGame(GameMatch gameMatch, Player player)
  {
    if (ClockService is not null)
    {
      ClockService.Stop();
      ClockService.Reset();
      ClockService.ShowClock(false);
    }
    Navigate("/game");
  }

  private void OnGameLeft()
  {
    Navigate("/");
  }

  private void Navigate(string location)
  {
    Match matcher = Regex.Match(Navigation?.Uri ?? "", @"(https|http):\/\/[a-zA-Z0-9.:]*\/([a-zA-Z0-9]*)[^\/]*$");
    string loc = matcher.Groups[2].Value;
    StopLoading();
    Console.WriteLine($"{loc}:{location}");
    Resolved = true;
    if (location == $"/{loc}")
    {
      StateHasChanged();
      return;
    }

    Navigation?.NavigateTo(location);
  }

  public void Dispose()
  {
    if (MatchService is not null)
    {
      MatchService.JoinGame -= HandleJoinGame;
      MatchService.OnGameLeft -= OnGameLeft;
    }

    if (Navigation is not null)
    {
      Navigation.LocationChanged -= HandleLocationChange;
    }
  }
}
