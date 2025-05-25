using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerConnectionContext
{
    public PlayerConnectionState State { get; private set; } = new EmptyState();
    public GameContext? GameContext { get; private set; } 
    private readonly ConnectionManager _connectionManager;
    public IHubContext<GameHub> Hub;

    public Player Player { get; set; }
    public Type Type => State.GetType();

    public PlayerConnectionContext(PlayerConnectionState state, Player player, ConnectionManager connectionManager, IHubContext<GameHub> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
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

    public Task<bool> Quit()
    {
        return State.Quit();
    }
    public Task<bool> JoinGame()
    {
        return State.JoinGame();
    }

    public async Task QuitGame()
    {

        TransitionTo(new PlayerLobby());
        await Task.Delay(100);
        await Hub.Clients.Client(Player.ConnectionId).SendAsync("leave_game");
        Console.WriteLine($"Player {Player} left the game");
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
    
    public virtual async Task<bool> Quit()
    {
        await Task.Delay(10);
        return false;
    }
}

public class EmptyState : PlayerConnectionState { }