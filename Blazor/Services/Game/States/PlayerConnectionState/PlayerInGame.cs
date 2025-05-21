using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerInGame(GameMatch match) : PlayerConnectionState
{
    private readonly GameMatch _match = match;

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = Context;
        ConnectionManager? connectionManager = ConnectionManager;
        IHubContext<GameHub>? hub = Clients;
        if (context is null || connectionManager is null || hub is null) { return; }

        hub.Clients.Client(context.Player.ConnectionId)
          .SendAsync("Join_game", _match, context.Player).GetAwaiter().OnCompleted(() =>
              {
                  Console.WriteLine($"Player {Context?.Player} Is Playing");
              });
    }

    public override async Task<bool> Disconnect()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = Context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerTempDisconnection(_match));
        return true;
    }


    public override async Task<bool> Quit()
    {
        await Task.Delay(50);
        ConnectionManager? connectionManager = ConnectionManager;
        if (connectionManager is null) { return false; }
        await connectionManager.EndGame(_match);
        return true;
    }

}