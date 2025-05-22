using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Pages.Game;

public partial class Game
{
    [Inject]
    private MatchService? MatchService { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    [Inject]
    private BoardService? Board { get; set; }


    public Player? Me { get; private set; }
    public Player? Opponent { get; private set; }

    private void HandleGameStateChange(GameMatch game)
    {
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (MatchService is null) { return; }
        MatchService.OnGameChange += HandleGameStateChange;

        await Task.CompletedTask;
    }

    private void LeaveGame()
    {
        if (Me is null) { return; }
        _ = MatchService?.LeaveGameAsync();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (MatchService is null) { return; }
        if (firstRender)
        {
            _ = Board?.InitAsync("game-board-container");
            // TODO send ready to play message.
        }
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (MatchService is null) { return; }
        MatchService.OnGameChange -= HandleGameStateChange;
    }
}