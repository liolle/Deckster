using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class GameContext
{
    const int TurnTimeLimit = 60;
    public GameState State { get; private set; }
    public GameMatch Match { get; }
    private readonly BoardManager _boardManager;
    private readonly IHubContext<GameHub> _hub;
    public readonly GameClockService GameClockService;
    
    public Type Type => State.GetType();

    public void TransitionTo(GameState state)
    {
        State = state;
        State.SetContext(this, _boardManager, _hub);
    }

    public GameContext(GameState state, GameMatch match,BoardManager boardManager, IHubContext<GameHub> hub)
    {
        State = state;
        Match = match;
        _boardManager = boardManager;
        _hub = hub;
        TransitionTo(state);
        GameClockService = new(TurnTimeLimit);
    }

    public Task<bool> PickPlayer()
    {
        return State.PickPlayer();
    }

    public Task<bool> PlayerReady(string playerId)
    {
        return State.PlayerReady(playerId);
    }
}

public abstract class GameState
{
    protected GameContext? Context;
    protected BoardManager? BoardManager;
    protected IHubContext<GameHub>?  Clients;
    
    protected GameState()
    {
        _ = AfterInit();
    }
    
    public async virtual Task AfterInit()
    {
        await Task.CompletedTask;
    }
    public void SetContext(GameContext context,BoardManager boardManager, IHubContext<GameHub> clients)
    {
        Context = context;
        BoardManager = boardManager;
        Clients = clients;
    }

    public virtual Task<bool> PickPlayer()
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> PlayerReady(string playerId)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> EndTurn(string playerId)
    {
        return Task.FromResult(false);
    }

    protected bool IsPlayerTurn(string playerId)
    {
        if (Context is null)
        {
            return false;
        }
        int? idx = Context.Match.NextToPlay;
        if (idx is null)
        {
            return false;
        }
        
        return Context.Match.Players[idx.Value].Id == playerId;
    }
}