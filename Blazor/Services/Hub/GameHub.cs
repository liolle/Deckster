using System.Security.Claims;
using Blazor.models;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services;

// Setup 

public partial class GameHub(
    ConnectionManager connectionManager,
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
        connectionManager.MatchPoll.TryGetValue(id, out GameContext? context);

        return context?.Match;
    }   
}


