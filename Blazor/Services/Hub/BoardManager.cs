using Blazor.models;
using Blazor.services.game.state;
using Blazor.utils;

namespace Blazor.services;

public interface IBoardManager
{
    public Task PickPlayer();
    public Task RegisterGame(GameContext context);
    public Task DeleteGame(string matchId);
}


public class BoardManager : IBoardManager
{
    public OwnedSemaphore MatchSemaphore { get; } = new(1, 1);
    public Dictionary<string, GameContext> MatchPoll { get; } = [];
    private Dictionary<string, string> MatchMapping { get; } = [];
    
    public Task PickPlayer()
    {
        return Task.CompletedTask;
    }

    public async Task RegisterGame(GameContext context)
    {
        Console.WriteLine($"Creating game: {context.Match.Id}");
        await MatchSemaphore.WaitAsync();
        MatchPoll.Add(context.Match.Id,context);
        MatchMapping.Add(context.Match.Player1.Id,context.Match.Id);
        MatchMapping.Add(context.Match.Player2.Id,context.Match.Id);
        MatchSemaphore.Release();
    }

    public async Task DeleteGame(string matchId)
    {
        Console.WriteLine($"DeleteGame: {matchId}");
        await MatchSemaphore.WaitAsync();
        GameContext context = MatchPoll.FirstOrDefault(val => val.Key == matchId).Value;
        MatchPoll.Remove(matchId);
        MatchMapping.Remove(context.Match.Player1.Id);
        MatchMapping.Remove(context.Match.Player2.Id);
        MatchSemaphore.Release();
    }

    public async Task<GameMatch?> GetPlayerMatch(string playerId)
    {
        await MatchSemaphore.WaitAsync();
        string gameId = MatchMapping.FirstOrDefault(val=> val.Key == playerId).Value;
        if (string.IsNullOrEmpty(gameId)) {return null;} 

        MatchSemaphore.Release();
        return MatchPoll.FirstOrDefault(val => val.Key == gameId).Value.Match;
    }
}