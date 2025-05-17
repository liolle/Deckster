using Blazor.models;
using Blazor.services.game;
using Microsoft.JSInterop;

namespace Blazor.services;

public interface IBoardService
{
    Task initAsync(string boar_container_name);
}

public class BoardService : IBoardService
{
    private readonly IJSRuntime _jsRuntime;

    private GameMatch _gameState { get; set; } = new(new(), new());

    private readonly MatchService _match;

    public BoardService(IJSRuntime jSRuntime, MatchService match)
    {
        _jsRuntime = jSRuntime;
        _match = match;
    }

    public async Task initAsync(string boar_container_name)
    {
        await _jsRuntime.InvokeVoidAsync("initializeBoard", boar_container_name);

        GameMatch? g = await _match.GetGameState();
        if (g is not null)
        {
            _gameState = g;
        }
        await _jsRuntime.InvokeVoidAsync("drawBoard", _gameState);
    }
}