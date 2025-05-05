using Blazor.models;
using Microsoft.JSInterop;

namespace Blazor.services.game;

public class MatchService : IDisposable
{
    public event Action<GameMatch, Player>? JoinGame;
    public event Func<Task>? OnStateChanged;
    public event Action? OnGameLeft;
    public event Action<GameMatch>? OnGameChange;
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<MatchService>? _dotNetObjectReference;

    public MatchService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        _jsRuntime.InvokeVoidAsync("initializeMatchService", _dotNetObjectReference);
    }


    [JSInvokable]
    public void NotifyJoinGame(GameMatch match, Player player)
    {
        JoinGame?.Invoke(match, player);
        OnStateChanged?.Invoke();
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
    }

    public async Task SearchGameAsync(int playerId)
    {
        await _jsRuntime.InvokeVoidAsync("searchGame", playerId);
    }

    public async Task LeaveGameAsync(int playerId)
    {
        await _jsRuntime.InvokeVoidAsync("leaveGame", playerId);
    }

    public async Task<GameMatch?> GetGameStateAsync()
    {
        return await _jsRuntime.InvokeAsync<GameMatch?>("getGameState");
    }

    public void Dispose()
    {
        _dotNetObjectReference?.Dispose();
    }
}