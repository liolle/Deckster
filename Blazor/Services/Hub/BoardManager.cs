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
            Console.WriteLine($"{key}: {unique}");
            if (!unique){return;}
            _ = Ready(playerId);
        }
        finally
        {
            ResetCallsKeys(key);
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

    public async Task DeleteGame(string matchId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        try
        {
            await MatchSemaphore.WaitAsync(randomId);
            GameContext context = MatchPoll.FirstOrDefault(val => val.Key == matchId).Value;
            MatchPoll.Remove(matchId);
            
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