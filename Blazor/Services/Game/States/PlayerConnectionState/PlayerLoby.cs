namespace Blazor.services.game.state;
public class PlayerLobby : PlayerConnectionState
{

    public override async Task AfterInit()
    {
        await base.AfterInit();
        Console.WriteLine($"Player {Context?.Player.Id} Entered the Lobby");
    }

    public override async Task<bool> SearchGame()
    {
        PlayerConnectionContext? context = Context;
        ConnectionManager? connectionManager = ConnectionManager;
        if (context is null || connectionManager is null) { return false; }
        await connectionManager.JoinQueueAsync(context.Player);
        context.TransitionTo(new PlayerSearching());
        return true;
    }

}