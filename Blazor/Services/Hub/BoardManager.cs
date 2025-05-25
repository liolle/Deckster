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
            Console.WriteLine($"{playerId} is ready to play");
        }
        finally
        {
            ResetCallsKeys(key);
        }
    }

    public async Task RegisterGame(GameContext context)
    {
        try
        {
            Console.WriteLine($"Creating game: {context.Match.Id}");
            await MatchSemaphore.WaitAsync();
            MatchPoll.Add(context.Match.Id,context);
            MatchMapping.Add(context.Match.Player1.Id,context.Match.Id);
            MatchMapping.Add(context.Match.Player2.Id,context.Match.Id);
        }
        finally
        {
            MatchSemaphore.Release();
        }
    }

    public async Task DeleteGame(string matchId)
    {
        try
        {
            Console.WriteLine($"DeleteGame: {matchId}");
            await MatchSemaphore.WaitAsync();
            GameContext context = MatchPoll.FirstOrDefault(val => val.Key == matchId).Value;
            MatchPoll.Remove(matchId);
            MatchMapping.Remove(context.Match.Player1.Id);
            MatchMapping.Remove(context.Match.Player2.Id);
        }
        finally
        {
            MatchSemaphore.Release();
        }
    }

    public async Task<GameMatch?> GetPlayerMatch(string playerId)
    {
        try
        {
            await MatchSemaphore.WaitAsync();
            string gameId = MatchMapping.FirstOrDefault(val=> val.Key == playerId).Value;
            if (string.IsNullOrEmpty(gameId)) {return null;} 
            return MatchPoll.FirstOrDefault(val => val.Key == gameId).Value.Match;
        }
        finally
        {
            MatchSemaphore.Release();
        }
    }

    private async Task<bool> IsUniqueCall(string key)
    {
        try
        {
            await CallsKeysSemaphore.WaitAsync();
            if (CallsKeys.Contains(key)){return false;}
            CallsKeys.Add(key);
            return true;
        }
        finally
        {
            CallsKeysSemaphore.Release();
        }
        return false;
    }

    private async void ResetCallsKeys(string key)
    {
        await Task.Delay(50 + Random.Shared.Next(0,50));
        await CallsKeysSemaphore.WaitAsync();
        CallsKeys.Remove(key);
        CallsKeysSemaphore.Release();
    }

    public void Dispose()
    {
        MatchSemaphore.Dispose();
    }
}