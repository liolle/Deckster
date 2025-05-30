using Blazor.models;
using Blazor.services.game.state;
using Blazor.utils;

namespace Blazor.services;

public interface IBoardManager
{
    public Task PickPlayer();
    public Task ReadyToPlay(string playerId);
    public Task RegisterGame(GameContext context);
    public Task DeleteGame(string matchId);
}


public class BoardManager : IBoardManager, IDisposable
{
    public OwnedSemaphore MatchSemaphore { get; } = new(1, 1,"BoardMatch");
    public OwnedSemaphore CallsKeysSemaphore { get; } = new(1, 1,"CallsKeys");
    public Dictionary<string, GameContext> MatchPoll { get; } = [];
    private Dictionary<string, string> MatchMapping { get; } = [];
    private HashSet<string> CallsKeys { get; } = new();
    
    public Task PickPlayer()
    {
        return Task.CompletedTask;
    }

    public async Task ReadyToPlay(string playerId)
    {
        string key = $"{playerId}:ReadyToPlay";
        try
        {
            bool unique = await IsUniqueCall(key);
            if (!unique){return;}
            _ = Ready(playerId);
        }
        finally
        {
            ResetCallsKeys(key);
        }
    }
    
    public async Task EndTurn(string playerId)
    {
        string key = $"{playerId}:EndTurn";
        try
        {
            bool unique = await IsUniqueCall(key);
            if (!unique){return;}
            _ = EndPlayerTurn(playerId);
        }
        finally
        {
            ResetCallsKeys(key);
        }
    }

    public async Task<bool> QuitGame(string playerId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        string? gameId = null;
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            MatchMapping.TryGetValue(playerId, out gameId);
            
            if (gameId is null){return false;}
            MatchPoll.TryGetValue(gameId, out GameContext? gContext);
            if (gContext is null){return false;}

            _ = gContext.QuitGame(playerId);
            return true;
        }
        finally
        {
            MatchSemaphore.Release(randomId);
            if (gameId is not null)
            {
                _ = DeleteGame(gameId);
            }
        }
    }

    private async Task EndPlayerTurn(string playerId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            MatchMapping.TryGetValue(playerId, out string? gameId);
            
            if (gameId is null){return;}
            MatchPoll.TryGetValue(gameId, out GameContext? matchContext);
            if (matchContext is null){return;}

            _ = matchContext.EndTurn(playerId);
        }
        finally
        {
            MatchSemaphore.Release(randomId);
        }
    }

    private async Task Ready(string playerId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            MatchMapping.TryGetValue(playerId, out string? gameId);
            
            if (gameId is null){return;}
            MatchPoll.TryGetValue(gameId, out GameContext? matchContext);
            if (matchContext is null){return;}

            _ = matchContext.PlayerReady(playerId);
        }
        finally
        {
            MatchSemaphore.Release(randomId);
        }
    }

    public async Task RegisterGame(GameContext context)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            MatchPoll.Add(context.Match.Id,context);
            MatchMapping.Add(context.Match.Players[0].Id,context.Match.Id);
            MatchMapping.Add(context.Match.Players[1].Id,context.Match.Id);
        }
        finally
        {
            MatchSemaphore.Release(randomId);
        }
    }
    
    /**
     * Responsible for the cleean the game instance from the match pool
     */
    public async Task DeleteGame(string matchId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            MatchPoll.TryGetValue(matchId, out GameContext? context);
            MatchPoll.Remove(matchId);
            if(context is null){return;}
            
            // Depending on the state we may need to release some event and or stoping the timer
            context.Dispose();
            
            MatchMapping.Remove(context.Match.Players[0].Id);
            MatchMapping.Remove(context.Match.Players[1].Id);
        }
        finally
        {
            MatchSemaphore.Release(randomId);
        }
    }

    public async Task<GameMatch?> GetPlayerMatch(string playerId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            string gameId = MatchMapping.FirstOrDefault(val=> val.Key == playerId).Value;
            if (string.IsNullOrEmpty(gameId)) {return null;} 
            return MatchPoll.FirstOrDefault(val => val.Key == gameId).Value.Match;
        }
        finally
        {
            MatchSemaphore.Release(randomId);
        }
    }

    private async Task<bool> IsUniqueCall(string key)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await CallsKeysSemaphore.WaitAsync(randomId);
            if (CallsKeys.Contains(key)){return false;}
            CallsKeys.Add(key);
            return true;
        }
        finally
        {
            CallsKeysSemaphore.Release(randomId);
        }
    }

    private async void ResetCallsKeys(string key)
    {
        int randomId = Random.Shared.Next(1000,10000);
        await Task.Delay(50 + Random.Shared.Next(0,50));
        await CallsKeysSemaphore.WaitAsync(randomId);
        CallsKeys.Remove(key);
        CallsKeysSemaphore.Release(randomId);
    }

    public void Dispose()
    {
        MatchSemaphore.Dispose();
    }
}