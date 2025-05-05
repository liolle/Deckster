using Blazor.models;

namespace Blazor.services.game.state;


public class PlayerSearching : PlayerConnectionState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();

        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;
        if (context is null || connectionManager is null) { return; }

        bool joined = await connectionManager.JoinQueueAsync(context.Player.Id);
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
        PlayerConnectionContext? context = _context;
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
        PlayerConnectionContext? context = _context;
        ConnectionManager? connectionManager = _connectionManager;

        if (context is null || connectionManager is null) { return false; }
        Player player = context.Player;
        if (player is null) { return false; }

        await connectionManager.Searching_semaphore.WaitAsync();
        connectionManager.Searching_poll.Remove(player.Id);
        connectionManager.Searching_semaphore.Release();

        context.TransitionTo(new PlayerLobby());
        return true;
    }
}