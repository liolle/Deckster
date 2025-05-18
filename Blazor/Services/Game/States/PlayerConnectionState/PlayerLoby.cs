namespace Blazor.services.game.state;
public class PlayerLobby : PlayerConnectionState
{

    public override async Task AfterInit()
    {
        await base.AfterInit();
        Console.WriteLine($"Player {_context?.Player.Id} Entered the Lobby");
    }

    public override async Task<bool> SearchGame()
    {
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return false; }
        await connectionManager.JoinQueueAsync(context.Player);
        context.TransitionTo(new PlayerSearching());
        return true;
    }

}