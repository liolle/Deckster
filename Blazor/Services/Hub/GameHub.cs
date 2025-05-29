using System.Security.Claims;
using Blazor.models;
using Blazor.services.game.state;
using Blazor.utils;
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


        int randomId = Random.Shared.Next(1000,10000);
        await connectionManager.PlayerPollSemaphore.WaitAsync(randomId);
        if (!connectionManager.PlayerPoll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            
            Player p = new()
            {
                Id = id,
                NickName = nickname
            };
            context = new(new PlayerLobby(), p,Context.ConnectionId, connectionManager, hubContext);
            p.SetContext(context);
            connectionManager.PlayerPoll.Add(id, context);
        }
        else
        {
            context.ConnectionId = Context.ConnectionId; 
        }
        connectionManager.PlayerPollSemaphore.Release(randomId);
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
        
        int randomId = Random.Shared.Next(1000,10000);

        try
        {
            await connectionManager.PlayerPollSemaphore.WaitAsync(randomId);
            if (!connectionManager.PlayerPoll.TryGetValue(id, out PlayerConnectionContext? context))
            {
                return;
            }

            await context.Disconnect();
            await base.OnDisconnectedAsync(exception);
        }
        finally
        {
            connectionManager.PlayerPollSemaphore.Release(randomId);
        }

    }
    
    public async Task SearchGameAsync(string playerId, string connectionId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        await connectionManager.PlayerPollSemaphore.WaitAsync(randomId);
        if (!connectionManager.PlayerPoll.TryGetValue(playerId, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), new Player() { Id = playerId },Context.ConnectionId, connectionManager, hubContext);
            connectionManager.PlayerPoll.Add(playerId, context);
        }
        _ = context.SearchGame();
        connectionManager.PlayerPollSemaphore.Release(randomId);
    }

    public async Task LeaveGameAsync(string playerId)
    {
        int randomId = Random.Shared.Next(1000,10000);
        await connectionManager.PlayerPollSemaphore.WaitAsync(randomId);
        connectionManager.PlayerPoll.TryGetValue(playerId, out PlayerConnectionContext? context);
        _ = context?.Quit();
        connectionManager.PlayerPollSemaphore.Release(randomId);
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

        return context.State.GetType().ToString().Split(".").Last() ;
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
        _ = boardManager.ReadyToPlay(id);
    }
}