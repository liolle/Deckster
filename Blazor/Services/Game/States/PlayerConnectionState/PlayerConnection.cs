using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerConnectionContext
{
    public PlayerConnectionState State { get; private set; } = new EmptyState();
    public GameContext? GameContext { get; private set; } 
    private readonly ConnectionManager _connectionManager;
    private IHubContext<GameHub> _hub;

    public Player Player { get; set; }
    public Type Type
    {
        get
        {
            return State.GetType();
        }
    }

    public void UpdateHub(IHubContext<GameHub> hub)
    {
        _hub = hub;
    }

    public PlayerConnectionContext(PlayerConnectionState state, Player player, ConnectionManager connectionManager, IHubContext<GameHub> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
        _hub = clients;
        TransitionTo(state);
    }

    public bool IsSameType(Type type)
    {
        return State.GetType() == type;
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        State = state;
        State.SetContext(this, _connectionManager, _hub);
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
        await _hub.Clients.Client(Player.ConnectionId).SendAsync("leave_game");
        Console.WriteLine($"Player {Player} left the game");
    }

}

public abstract class PlayerConnectionState
{
    protected PlayerConnectionContext? _context;
    protected ConnectionManager? _connectionManager;
    protected IHubContext<GameHub>? _clients;

    public PlayerConnectionState()
    {
        _ = AfterInit();
    }

    public async virtual Task AfterInit()
    {
        await Task.Delay(20);
    }

    public void SetContext(PlayerConnectionContext context, ConnectionManager connectionManager, IHubContext<GameHub> clients)
    {
        _context = context;
        _connectionManager = connectionManager;
        _clients = clients;
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