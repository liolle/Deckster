using Blazor.models;

namespace Blazor.services.game.state;


public class PlayerSearching : PlayerConnectionState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();

        PlayerConnectionContext? context = Context;
        ConnectionManager? connectionManager = ConnectionManager;
        if (context is null || connectionManager is null) { return; }

        bool joined = await connectionManager.JoinQueueAsync(context.Player);
        if (joined)
        {
            Console.WriteLine($"Player {context.Player.Id} Is searching for a match");
            Random random = new();
            int delay = random.Next(500) + 500;
            await Task.Delay(delay);
            _ = connectionManager.FindMatchUp();
        }
    }

    public override async Task<bool> MatchFound(GameMatch match)
    {
        await Task.Delay(50);
        PlayerConnectionContext? context = Context;
        if (context is null) { return false; }
        context.TransitionTo(new PlayerMathFound(match));
        return true;
    }

    public override async Task<bool> Quit()
    {
        return await Disconnect();
    }

    public override async Task<bool> Disconnect()
    {
        PlayerConnectionContext? context = Context;
        ConnectionManager? connectionManager = ConnectionManager;

        if (context is null || connectionManager is null) { return false; }
        Player player = context.Player;

        await connectionManager.SearchingSemaphore.WaitAsync();
        connectionManager.SearchingPoll.Remove(player);
        connectionManager.SearchingSemaphore.Release();

        context.TransitionTo(new PlayerLobby());
        return true;
    }
}