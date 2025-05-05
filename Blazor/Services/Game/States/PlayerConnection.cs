using Blazor.models;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services.game.state;

public class PlayerConnectionContext
{
    public PlayerConnectionState _state { get; private set; } = new EmptyState();
    private readonly ConnectionManager _connectionManager;
    private IHubContext<GameHubService> _hub;

    public Player Player { get; set; }
    public Type Type
    {
        get
        {
            return _state.GetType();
        }
    }

    public void UpdateHub(IHubContext<GameHubService> hub)
    {
        _hub = hub;
    }

    public PlayerConnectionContext(PlayerConnectionState state, Player player, ConnectionManager connectionManager, IHubContext<GameHubService> clients)
    {
        _connectionManager = connectionManager;
        Player = player;
        _hub = clients;
        TransitionTo(state);
    }

    public bool IsSameType(Type type)
    {
        return _state.GetType() == type;
    }

    public void TransitionTo(PlayerConnectionState state)
    {
        _state = state;
        _state.SetContext(this, _connectionManager, _hub);
    }

    public Task<bool> SearchGame()
    {
        return _state.SearchGame();
    }

    public Task<bool> MatchFound(GameMatch match)
    {
        return _state.MatchFound(match);
    }

    public Task<bool> Disconnect()
    {
        return _state.Disconnect();
    }

    public Task<bool> Quit()
    {
        return _state.Quit();
    }
    public Task<bool> JoinGame()
    {
        return _state.JoinGame();
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
    protected IHubContext<GameHubService>? _clients;

    public PlayerConnectionState()
    {
        _ = AfterInit();
    }

    public async virtual Task AfterInit()
    {
        await Task.Delay(20);
    }

    public void SetContext(PlayerConnectionContext context, ConnectionManager connectionManager, IHubContext<GameHubService> clients)
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