using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class GameContext
{
    public GameState State { get; private set; }
    public GameMatch Match { get; }
    
    public Type Type
    {
        get
        {
            return State.GetType();
        }
    }

    public void TransitionTo(GameState state)
    {
        State = state;
        //State.SetContext(this, _connectionManager, _hub);
    }

    public GameContext(GameState state, GameMatch match)
    {
        State = state;
        Match = match;
    }

    public Task<bool> PickPlayer()
    {
        return State.PickPlayer();
    }
}

public abstract class GameState
{
    protected GameContext? Context;
    protected IHubContext<GameHub>?  Clients;
    
    protected GameState()
    {
        _ = AfterInit();
    }

    
    public async virtual Task AfterInit()
    {
        await Task.CompletedTask;
    }
    public void SetContext(GameContext context, IHubContext<GameHub> clients)
    {
        Context = context;
        Clients = clients;
    }

    public virtual Task<bool> PickPlayer()
    {
        return Task.FromResult(false);
    }
    
    
    
    
}