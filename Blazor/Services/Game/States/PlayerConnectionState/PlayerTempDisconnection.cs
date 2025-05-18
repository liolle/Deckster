using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerTempDisconnection(GameMatch match) : PlayerConnectionState
{
    private readonly GameMatch match = match;

    public override async Task AfterInit()
    {
        await base.AfterInit();
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        IHubContext<GameHub>? hub = _clients;
        if (context is null || connectionManager is null || hub is null) { return; }

        hub.Clients.Client(context.Player.ConnectionId)
          .SendAsync("Join_game", match, context.Player).GetAwaiter().OnCompleted(() =>
              {
                  Console.WriteLine($"Player {_context?.Player.Id} Lost connection");
              });
    }

    public override async Task<bool> SearchGame()
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = _context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerPlaying(match));
        return true;
    }
}