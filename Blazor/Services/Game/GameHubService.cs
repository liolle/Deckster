using System.Security.Claims;
using Blazor.models;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Blazor.services;

public class GameHubService(ConnectionManager connectionManager, AuthenticationStateProvider authProvider, IHubContext<GameHubService> hubContext) : Hub
{
    private async Task<IEnumerable<Claim>> GetClaims()
    {
        return (await authProvider.GetAuthenticationStateAsync()).User.Claims;
    }

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

        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.Player_poll.Add(id, context);
        }
        else
        {
            context.Player = p;
        }
        connectionManager.Player_poll_semaphore.Release();
        if (context.IsSameType(typeof(PlayerTempDisconnection)))
        {
            await context.SearchGame();
        }
    }

    public async Task SearchGameAsync(string playerId, string connectionId)
    {
        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), new Player() { Id = playerId, ConnectionId = connectionId }, connectionManager, hubContext);
            connectionManager.Player_poll.Add(playerId, context);
        }
        _ = context.SearchGame();
        connectionManager.Player_poll_semaphore.Release();
    }

    public async Task LeaveGameAsync(string playerId)
    {
        await connectionManager.Player_poll_semaphore.WaitAsync();
        connectionManager.Player_poll.TryGetValue(playerId, out PlayerConnectionContext? context);
        _ = context?.Quit();
        connectionManager.Player_poll_semaphore.Release();
    }


    public async Task<string> GetPlayerStateAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return ""; }
        connectionManager.Player_poll.TryGetValue(id, out PlayerConnectionContext? context);
        if (context is null)
        {
            return "";
        }
        return context._state.GetType().ToString();
    }

    public async Task<GameMatch?> GetGameStateAsync()
    {
        IEnumerable<Claim> claims = await GetClaims();
        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return null; }
        connectionManager.Match_poll.TryGetValue(id, out GameMatch? match);

        return match;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        IEnumerable<Claim> claims = await GetClaims();

        string? id = claims.FirstOrDefault(val => val.Type == "Id")?.Value;
        if (id is null) { return; }

        Player p = new() { Id = id, ConnectionId = Context.ConnectionId };
        await connectionManager.Player_poll_semaphore.WaitAsync();
        if (!connectionManager.Player_poll.TryGetValue(id, out PlayerConnectionContext? context))
        {
            context = new(new PlayerLobby(), p, connectionManager, hubContext);
            connectionManager.Player_poll.Add(id, context);
        }
        connectionManager.Player_poll_semaphore.Release();

        await context.Disconnect();
        await base.OnDisconnectedAsync(exception);
    }
}