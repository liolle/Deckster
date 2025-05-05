using System.Security.Claims;
using Blazor.models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Blazor.services.game;

public partial class MatchService : IDisposable
{
    public event Action<GameMatch, Player>? JoinGame;
    public event Func<Task>? OnStateChanged;
    public event Action? OnGameLeft;
    public event Action<GameMatch>? OnGameChange;
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<MatchService>? _dotNetObjectReference;

    private AuthenticationStateProvider? _authProvider { get; set; }

    private string UserId { get; set; } = "";
    public bool Searching { get; private set; } = false;

    public MatchService(IJSRuntime jSRuntime, AuthenticationStateProvider authProvider)
    {
        _jsRuntime = jSRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        _jsRuntime.InvokeVoidAsync("initializeMatchService", _dotNetObjectReference);
        _authProvider = authProvider;
        InitAsync();
    }

    private async void InitAsync()
    {
        if (_authProvider is not null)
        {
            AuthenticationState authState = await _authProvider.GetAuthenticationStateAsync();
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
        Searching = false;
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
        Searching = false;
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
    }

    public async Task LeaveGameAsync()
    {
        if (string.IsNullOrEmpty(UserId))
        {
            return;
        }
        await _jsRuntime.InvokeVoidAsync("leaveGame", UserId);
        Searching = false;
    }

    public async Task<GameMatch?> GetGameStateAsync()
    {
        return await _jsRuntime.InvokeAsync<GameMatch?>("getGameState");
    }
}