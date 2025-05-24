using Blazor.models;
using Blazor.services.game;
using Microsoft.JSInterop;

namespace Blazor.services;

public interface IBoardService
{
    Task InitAsync(string boarContainerName);
}

public class BoardService : IBoardService
{
    private readonly IJSRuntime _jsRuntime;

    private GameMatch GameState { get; set; } = new(new(), new());

    private readonly MatchService _match;

    public BoardService(IJSRuntime jSRuntime, MatchService match)
    {
        _jsRuntime = jSRuntime;
        _match = match;
    }

    public async Task InitAsync(string boarContainerName)
    {
        await _jsRuntime.InvokeVoidAsync("initializeBoard", boarContainerName);
    }

    public async Task DrawBoard(Player player)
    {
        await UpdateGameState();
        await _jsRuntime.InvokeVoidAsync("drawBoard", GameState, player.Id);
    }

    public async Task UpdateGameState()
    {
        GameMatch? m = await _match.GetGameState();
        if (m is null){return;}
        GameState = m;
    }

    public void ReadyToPlay()
    {
       _ = _jsRuntime.InvokeVoidAsync("readyToPlay");
    }
}