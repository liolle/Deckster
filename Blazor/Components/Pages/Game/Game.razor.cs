using Blazor.models;
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

    public GameMatch? _game { get; private set; } = new GameMatch(new(), new());

    public Player? _me { get; private set; }
    public Player? _opponent { get; private set; }

    private void HandleGameStateChange(GameMatch game)
    {
        _game = game;
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(10);
        if (_matchService is null) { return; }
        _matchService.OnGameChange += HandleGameStateChange;
    }

    private async Task SetPlayers()
    {
        if (_game is null)
        { return; }

        if (_authProvider is null || _game is null) { return; }
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? id = user.FindFirst("Id")?.Value;
        if (id is null) { return; }
        if (_game.player1.Id == id)
        {
            _opponent = _game.player2;
            _me = _game.player1;
        }
        else
        {
            _opponent = _game.player1;
            _me = _game.player2;
        }

        await Task.CompletedTask;
        StateHasChanged();
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
            GameMatch? g = await _matchService.GetGameState();
            if (g is not null)
            {
                _game = g;
            }

            _ = SetPlayers();
        }
    }

    public void Dispose()
    {
        if (_matchService is null) { return; }
        _matchService.OnGameChange -= HandleGameStateChange;
    }
}