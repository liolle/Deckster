using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class GameContext
{
    public GameState State { get; }
    
    public Type Type
    {
        get
        {
            return State.GetType();
        }
    }

    public GameContext(GameState state)
    {
        State = state;
    }
}

public abstract class GameState
{
    protected GameContext? _context;
    protected IHubContext<GameHub>? _clients;
    
    public GameState()
    {
        _ = AfterInit();
    }

    public async virtual Task AfterInit()
    {
        await Task.CompletedTask;
    }
    
    // TODO Add actions to pass from states to change state
    
}