using System.Security.Claims;
using Blazor.models;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Blazor.services.game;

public enum MatchState
{
    lobby,
    searching,
    playing,
}

/*
 * Scoped service facilitating communication with the GameHub 
 */
public partial class MatchService : IDisposable
{
    public event Action<GameMatch, Player>? JoinGame;
    public event Func<Task>? OnStateChanged;
    public event Action? OnGameLeft;
    public event Action<GameMatch>? OnGameChange;
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<MatchService>? _dotNetObjectReference;

    private AuthenticationStateProvider? AuthProvider { get; set; }

    private string UserId { get; set; } = "";
    public MatchState State { get; set; } = MatchState.lobby;
    public bool IsLoading { get; set; } = true;

    public MatchService(IJSRuntime jSRuntime, AuthenticationStateProvider authProvider)
    {
        _jsRuntime = jSRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        _jsRuntime.InvokeVoidAsync("initializeMatchService", _dotNetObjectReference);
        AuthProvider = authProvider;
        InitAsync();
    }

    private async void InitAsync()
    {
        if (AuthProvider is not null)
        {
            AuthenticationState authState = await AuthProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal principal = authState.User;

            if (!(principal.Identity?.IsAuthenticated ?? false)) { return; }

            UserId = principal.FindFirst("Id")?.Value ?? "";

        }
    }

    public void Dispose()
    {
        _dotNetObjectReference?.Dispose();
    }
}

// JS => Dotnet function calls 
public partial class MatchService
{
    [JSInvokable]
    public void NotifyJoinGame(GameMatch match, Player player)
    {
        JoinGame?.Invoke(match, player);
        OnStateChanged?.Invoke();
        State = MatchState.playing;
    }

    [JSInvokable]
    public void GameHasChanged(GameMatch match)
    {
        OnGameChange?.Invoke(match);
    }

    [JSInvokable]
    public void NotifyLeftGame()
    {
        OnGameLeft?.Invoke();
        State = MatchState.lobby;
    }

    [JSInvokable]
    public string GetUserId()
    {
        return UserId;
    }

}

// Dotnet => Js function calls
public partial class MatchService
{
    public async Task SearchGameAsync()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync("searchGame", UserId);
        State = MatchState.searching;
    }

    public async Task LeaveGameAsync()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            return;
        }
        await _jsRuntime.InvokeVoidAsync("leaveGame", UserId);
        State = MatchState.lobby;
    }

    public async Task<string> GetPlayerState()
    {
        return await _jsRuntime.InvokeAsync<string>("getPlayerState");
    }

    public async Task<GameMatch?> GetGameState()
    {
        return await _jsRuntime.InvokeAsync<GameMatch?>("getGameState");
    }
}