using System.Security.Claims;
using Blazor.models;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services;

// Setup 
/*
 * Singleton service responsible for managing SignalR messages (we need to use the connection manager to modify the player context)
 * Those methods are called on the client using the signalrR js library 
 */
public partial class GameHub(
    ConnectionManager connectionManager,
    BoardManager boardManager,
    AuthenticationStateProvider authProvider,
    IHubContext<GameHub> hubContext) : Hub
{
    private async Task<IEnumerable<Claim>> GetClaims()
    {
        return (await authProvider.GetAuthenticationStateAsync()).User.Claims;
    }
}
// Player connection methods
public partial class GameHub
{
    public override async Task OnConnectedAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        string? nickname = claims.FirstOrDefault(val => val.Type == "NickName")?.Value;
        if (id is null || nickname is null) { return; }

        Player p = new()
        {
            Id = id,
            ConnectionId = Context.ConnectionId,
            NickName = nickname
        };

        await connectionManager.PlayerPollSemaphore.WaitAsync();
        if (!connectionManager.PlayerPoll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.PlayerPoll.Add(id, context);
        }
        else
        {
            context.Player = p;
        }
        connectionManager.PlayerPollSemaphore.Release();
        if (context.IsSameType(typeof(PlayerTempDisconnection)))
        {
            await context.SearchGame();
        }
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        IEnumerable<Claim> claims = await GetClaims();

        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return; }

        Player p = new() { Id = id, ConnectionId = Context.ConnectionId };
        await connectionManager.PlayerPollSemaphore.WaitAsync();
        if (!connectionManager.PlayerPoll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.PlayerPoll.Add(id, context);
        }
        connectionManager.PlayerPollSemaphore.Release();

        await context.Disconnect();
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SearchGameAsync(string playerId, string connectionId)
    {
        await connectionManager.PlayerPollSemaphore.WaitAsync();
        if (!connectionManager.PlayerPoll.TryGetValue(playerId, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), new Player() { Id = playerId, ConnectionId = connectionId }, connectionManager, hubContext);
            connectionManager.PlayerPoll.Add(playerId, context);
        }
        _ = context.SearchGame();
        connectionManager.PlayerPollSemaphore.Release();
    }

    public async Task LeaveGameAsync(string playerId)
    {
        await connectionManager.PlayerPollSemaphore.WaitAsync();
        connectionManager.PlayerPoll.TryGetValue(playerId, out PlayerConnectionContext? context);
        _ = context?.Quit();
        connectionManager.PlayerPollSemaphore.Release();
    }
    
    public async Task<string> GetPlayerStateAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return ""; }
        connectionManager.PlayerPoll.TryGetValue(id, out PlayerConnectionContext? context);
        if (context is null)
        {
            return "";
        }
        return context.State.GetType().ToString();
    }
    
}

// Game methods
public partial class GameHub
{
    public async Task<GameMatch?> GetGameStateAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return null; }

        return await boardManager.GetPlayerMatch(id);
    }   
    
    public async Task ReadyToPlayAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null)
        {
            return;
        }
        
        // get the game context base on the player 
        // return if not found

        
        // Send Player ready transformation
        
        Console.WriteLine($"PlayerId: {id} is ready to play");
    }   
}


