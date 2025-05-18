using Blazor.models;
using Blazor.services.game.state;
using Blazor.utils;

namespace Blazor.services;

public interface IConnectionManager
{
    public Task<bool> JoinQueueAsync(Player p);
    public Task EndGame(GameMatch game);
    public Task<string> GetPlayerState(string player_id);
}

public class ConnectionManager : IConnectionManager
{
    public OwnedSemaphore Searching_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Player_poll_semaphore { get; } = new(1, 1);
    public OwnedSemaphore Match_semaphore { get; } = new(1, 1);
    public HashSet<Player> Searching_poll { get; } = [];
    public Dictionary<string, PlayerConnectionContext> Player_poll { get; } = [];
    public Dictionary<string, GameMatch> Match_poll { get; } = [];

    public async Task<bool> JoinQueueAsync(Player p)
    {
        await Searching_semaphore.WaitAsync();
        Searching_poll.Add(p);
        Searching_semaphore.Release();
        return true;
    }

    public async Task FindMatchUp()
    {
        await Searching_semaphore.WaitAsync();
        if (Searching_poll.Count < 2) { return; }
        Random random = new();

        List<Player> players = [.. Searching_poll];
        int p1 = random.Next(players.Count);
        int p2;
        do
        {
            p2 = random.Next(players.Count);
        } while (p1 == p2);

        Player player1 = players[p1];
        Player player2 = players[p2];

        Searching_poll.Remove(player1);
        Searching_poll.Remove(player2);
        await CreateMatch(player1, player2);
        Searching_semaphore.Release();
    }

    private async Task CreateMatch(Player playerId1, Player playerId2)
    {
        await Player_poll_semaphore.WaitAsync();
        Player_poll.TryGetValue(playerId1.Id, out PlayerConnectionContext? p1Context);
        Player_poll.TryGetValue(playerId2.Id, out PlayerConnectionContext? p2Context);
        Player_poll_semaphore.Release();

        if (p1Context is null || p2Context is null)
        {
            p2Context?.Disconnect();
            p1Context?.Disconnect();
            return;
        }

        GameMatch match = new(playerId1, playerId2);

        await Match_semaphore.WaitAsync();
        Match_poll.Add(playerId1.Id, match);
        Match_poll.Add(playerId2.Id, match);
        Match_semaphore.Release();

        _ = p1Context.MatchFound(match);
        _ = p2Context.MatchFound(match);
    }

    public async Task EndGame(GameMatch match)
    {
        await Match_semaphore.WaitAsync();
        Match_poll.Remove(match.Player1.Id);
        Match_poll.Remove(match.Player2.Id);
        Match_semaphore.Release();

        await Player_poll_semaphore.WaitAsync();

        Player_poll.TryGetValue(match.Player1.Id, out PlayerConnectionContext? context_p1);
        Player_poll.TryGetValue(match.Player2.Id, out PlayerConnectionContext? context_p2);

        context_p1?.QuitGame();
        context_p2?.QuitGame();

        Player_poll_semaphore.Release();
    }

    public async Task<string> GetPlayerState(string player_id)
    {
        await Player_poll_semaphore.WaitAsync();
        PlayerConnectionState state = Player_poll.FirstOrDefault(val => val.Key == player_id).Value.State;
        Player_poll_semaphore.Release();

        return state.GetType().Name;
    }
};