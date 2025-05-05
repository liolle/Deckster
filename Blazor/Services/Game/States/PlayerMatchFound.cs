using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;


public class PlayerMathFound(GameMatch match) : PlayerConnectionState
{
    private readonly GameMatch _match = match;

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        IHubContext<GameHubService>? clients = _clients;
        if (context is null || connectionManager is null || clients is null) { return; }
        Console.WriteLine($"Player {context.Player.id} Found a match \n- {_match}");
        await JoinGame();
    }

    public override async Task<bool> JoinGame()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerPlaying(_match));
        return true;
    }

    public override async Task<bool> Disconnect()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerTempDisconnection(_match));
        return true;
    }
}