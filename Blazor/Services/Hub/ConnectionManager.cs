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

/*
 * Singleton service responsible for holding and managing playerContext.
 */
public class ConnectionManager(BoardManager boardManager) : IConnectionManager,IDisposable
{
    public OwnedSemaphore SearchingSemaphore { get; } = new(1, 1,"Searching");
    public OwnedSemaphore PlayerPollSemaphore { get; } = new(1, 1,"PlayerPoll");
    public HashSet<Player> SearchingPoll { get; } = [];
    public Dictionary<string, PlayerConnectionContext> PlayerPoll { get; } = [];
    
    public async Task<bool> JoinQueueAsync(Player p)
    {
        try
        {
            await SearchingSemaphore.WaitAsync();
            SearchingPoll.Add(p);
            return true;
        }
        finally
        {
            
            SearchingSemaphore.Release();
        }
    }
    

    public async Task FindMatchUp()
    {
        try
        {
            await SearchingSemaphore.WaitAsync();
            if (SearchingPoll.Count < 2)
            {
                SearchingSemaphore.Release();
                return;
            }
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
        }
        finally
        {
            
            SearchingSemaphore.Release();
        }
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
        GameContext gameContext = new GameContext(new GameInit(),match,new BoardManager(),p1Context.Hub);
        

        await boardManager.RegisterGame(gameContext);

        _ = p1Context.MatchFound(match);
        _ = p2Context.MatchFound(match);
        
    }

    public async Task EndGame(GameMatch match)
    {
        await boardManager.DeleteGame(match.Id);
        await PlayerPollSemaphore.WaitAsync();
        try { 
            PlayerPoll.TryGetValue(match.Player1.Id, out PlayerConnectionContext? contextP1); 
            PlayerPoll.TryGetValue(match.Player2.Id, out PlayerConnectionContext? contextP2);

            if (contextP1 is null || contextP2 is null)
            {
                return;
            }

            _ = contextP1.QuitGame();
            _ =  contextP2.QuitGame();
        }
        finally
        {
            PlayerPollSemaphore.Release();
        }
    }

    public async Task<string> GetPlayerState(string playerId)
    {
        await PlayerPollSemaphore.WaitAsync();
        PlayerPoll.TryGetValue(playerId, out PlayerConnectionContext? context) ;
        if (context is null)
        {
            return "";
        }
        PlayerPollSemaphore.Release();

        return nameof(context.State);
    }

    public void Dispose()
    {
        boardManager.Dispose();
        SearchingSemaphore.Dispose();
        PlayerPollSemaphore.Dispose();
    }
   
};