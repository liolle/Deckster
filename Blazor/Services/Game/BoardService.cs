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

        GameMatch? g = await _match.GetGameState();
        if (g is not null)
        {
            GameState = g;
        }
        await _jsRuntime.InvokeVoidAsync("drawBoard", GameState);
    }
}