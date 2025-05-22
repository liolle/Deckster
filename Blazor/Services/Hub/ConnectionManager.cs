using Blazor.models;
using Blazor.services.game.state;
using Blazor.utils;

namespace Blazor.services;

public interface IConnectionManager
{
    public Task<bool> JoinQueueAsync(Player p);
    public Task EndGame(GameMatch game);
    public Task<string> GetPlayerState(string playerId);
}

public class ConnectionManager : IConnectionManager
{
    public OwnedSemaphore SearchingSemaphore { get; } = new(1, 1);
    public OwnedSemaphore PlayerPollSemaphore { get; } = new(1, 1);
    public OwnedSemaphore MatchSemaphore { get; } = new(1, 1);
    public HashSet<Player> SearchingPoll { get; } = [];
    public Dictionary<string, PlayerConnectionContext> PlayerPoll { get; } = [];
    public Dictionary<string, GameContext> MatchPoll { get; } = [];

    public async Task<bool> JoinQueueAsync(Player p)
    {
        await SearchingSemaphore.WaitAsync();
        SearchingPoll.Add(p);
        SearchingSemaphore.Release();
        return true;
    }

    public async Task FindMatchUp()
    {
        await SearchingSemaphore.WaitAsync();
        if (SearchingPoll.Count < 2) { return; }
        Random random = new();

        List<Player> players = [.. SearchingPoll];
        int p1 = random.Next(players.Count);
        int p2;
        do
        {
            p2 = random.Next(players.Count);
        } while (p1 == p2);

        Player player1 = players[p1];
        Player player2 = players[p2];

        SearchingPoll.Remove(player1);
        SearchingPoll.Remove(player2);
        await CreateMatch(player1, player2);
        SearchingSemaphore.Release();
    }

    private async Task CreateMatch(Player playerId1, Player playerId2)
    {
        await PlayerPollSemaphore.WaitAsync();
        PlayerPoll.TryGetValue(playerId1.Id, out PlayerConnectionContext? p1Context);
        PlayerPoll.TryGetValue(playerId2.Id, out PlayerConnectionContext? p2Context);
        PlayerPollSemaphore.Release();

        if (p1Context is null || p2Context is null)
        {
            p2Context?.Disconnect();
            p1Context?.Disconnect();
            return;
        }

        GameMatch match = new(playerId1, playerId2);
        GameContext context = new GameContext(new GameInit(),match,new BoardManager(),p1Context.Hub);
        

        await MatchSemaphore.WaitAsync();
        MatchPoll.Add(playerId1.Id, context);
        MatchPoll.Add(playerId2.Id, context);
        MatchSemaphore.Release();

        _ = p1Context.MatchFound(match);
        _ = p2Context.MatchFound(match);
    }

    public async Task EndGame(GameMatch match)
    {
        await MatchSemaphore.WaitAsync();
        MatchPoll.Remove(match.Player1.Id);
        MatchPoll.Remove(match.Player2.Id);
        MatchSemaphore.Release();

        await PlayerPollSemaphore.WaitAsync();

        PlayerPoll.TryGetValue(match.Player1.Id, out PlayerConnectionContext? contextP1);
        PlayerPoll.TryGetValue(match.Player2.Id, out PlayerConnectionContext? contextP2);

        contextP1?.QuitGame();
        contextP2?.QuitGame();

        PlayerPollSemaphore.Release();
    }

    public async Task<string> GetPlayerState(string playerId)
    {
        await PlayerPollSemaphore.WaitAsync();
        PlayerConnectionState state = PlayerPoll.FirstOrDefault(val => val.Key == playerId).Value.State;
        PlayerPollSemaphore.Release();

        return state.GetType().Name;
    }
};