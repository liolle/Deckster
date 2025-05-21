using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Pages.Game;

public partial class Game
{
    [Inject]
    private MatchService? _matchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? _authProvider { get; set; }

    [Inject]
    private BoardService? _board { get; set; }


    public Player? _me { get; private set; }
    public Player? _opponent { get; private set; }

    private void HandleGameStateChange(GameMatch game)
    {
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (_matchService is null) { return; }
        _matchService.OnGameChange += HandleGameStateChange;

        await Task.CompletedTask;
    }

    private void LeaveGame()
    {
        if (_me is null) { return; }
        _ = _matchService?.LeaveGameAsync();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_matchService is null) { return; }
        if (firstRender)
        {
            _ = _board?.InitAsync("game-board-container");
        }
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_matchService is null) { return; }
        _matchService.OnGameChange -= HandleGameStateChange;
    }
}