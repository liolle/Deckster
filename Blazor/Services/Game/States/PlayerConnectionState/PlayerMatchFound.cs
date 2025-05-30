using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;


public class PlayerMathFound(GameMatch match) : PlayerConnectionState
{
    private readonly GameMatch _match = match;

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = Context;
        ConnectionManager? connectionManager = ConnectionManager;
        IHubContext<GameHub>? clients = Clients;
        if (context is null || connectionManager is null || clients is null) { return; }
        Console.WriteLine($"Player {context.Player.Id} Found a match");
        await JoinGame();
    }

    public override async Task<bool> JoinGame()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = Context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerInGame(_match));
        return true;
    }

    public override async Task<bool> Disconnect()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = Context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerTempDisconnection(_match));
        return true;
    }
}