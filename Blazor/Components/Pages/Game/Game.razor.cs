using System.Security.Claims;
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

    private string _myId = "";
    public Player? Me { get; private set; }
    public Player? Opponent { get; private set; }

    private void HandleGameStateChange(GameMatch game)
    {
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (MatchService is null || AuthProvider is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync(); 
        IEnumerable<Claim> claims = authState.User.Claims;
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        _myId = id!;
        
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
        if (!firstRender){return;}
        if (MatchService is null) { return; }
        {
            await SetPlayer();
            if (Board is null || Me is null) { return; }
            await Board.InitAsync("game-board-container");
            await Board.DrawBoard(Me);
            Board.ReadyToPlay();
        }
    }

    private async Task SetPlayer()
    {
        if (MatchService is null) { return; }
        GameMatch? m = await MatchService.GetGameState();
        Console.WriteLine(m?.Id ?? "-->");
        if (m is null || string.IsNullOrEmpty(_myId)){return;}
        if (m.Player1.Id == _myId)
        {
            Me = m.Player1;
            Opponent = m.Player2;
        }
        else
        {
            Opponent = m.Player1;
            Me = m.Player2;
        } 
    }

    public void Dispose()
    {
        if (MatchService is null) { return; }
        MatchService.OnGameChange -= HandleGameStateChange;
    }
}