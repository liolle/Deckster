using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerConnectionContext
{
    public PlayerConnectionState State { get; private set; } = new EmptyState();
    public GameContext? GContext { get; private set; } 
    private readonly ConnectionManager _connectionManager;
    public IHubContext<GameHub> Hub;
    public string ConnectionId { get; set; } = "";

    public Player Player { get; init; }
    public Type Type => State.GetType();

    public PlayerConnectionContext(PlayerConnectionState state, Player player,string connectionId, ConnectionManager connectionManager, IHubContext<GameHub> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
        ConnectionId = connectionId;
        Player.SetContext(this);
        Hub = clients;
        TransitionTo(state);
    }

    public bool IsSameType(Type type)
    {
        return State.GetType() == type;
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        State = state;
        State.SetContext(this, _connectionManager, Hub);
    }

    public Task<bool> SearchGame()
    {
        return State.SearchGame();
    }

    public Task<bool> MatchFound(GameMatch match)
    {
        return State.MatchFound(match);
    }

    public Task<bool> Disconnect()
    {
        return State.Disconnect();
    }
   
    public Task<bool> JoinGame()
    {
        return State.JoinGame();
    }

    public Task<bool> LeaveGame()
    {
        return State.LeaveGame();
    }

    public Task<bool> QuitGame()
    {
        return State.QuitGame();
    }
}

public abstract class PlayerConnectionState
{
    protected PlayerConnectionContext? Context;
    protected ConnectionManager? ConnectionManager;
    protected IHubContext<GameHub>? Clients;

    protected PlayerConnectionState()
    {
        _ = AfterInit();
    }

    public virtual async Task AfterInit()
    {
        await Task.Delay(20);
    }

    public void SetContext(PlayerConnectionContext context, ConnectionManager connectionManager, IHubContext<GameHub> clients)
    {
        Context = context;
        ConnectionManager = connectionManager;
        Clients = clients;
    }

    public virtual async Task<bool> Disconnect()
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> JoinGame()
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> MatchFound(GameMatch match)
    {
        await Task.Delay(10);
        return false;
    }

    public virtual async Task<bool> SearchGame()
    {
        await Task.Delay(10);
        return false;
    }

    public async Task<bool> LeaveGame()
    {
        IHubContext<GameHub>? hub = Clients;
        if (Context is null || hub is null) { return false; }
        
        Context.TransitionTo(new PlayerLobby());

        await hub.Clients.Client(Context.Player.ConnectionId)
            .SendAsync("leave_game");
        return true; 
    }

    public virtual async Task<bool> QuitGame()
    {
        await Task.Delay(10);
        return false;
    }
}

public class EmptyState : PlayerConnectionState { }