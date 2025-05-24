using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class GameContext
{
    public GameState State { get; private set; }
    public GameMatch Match { get; }
    private readonly BoardManager _boardManager;
    private readonly IHubContext<GameHub> _hub;
    
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
    }

    public Task<bool> PickPlayer()
    {
        return State.PickPlayer();
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
    
    
    
    
}