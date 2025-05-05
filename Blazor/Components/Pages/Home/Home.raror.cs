using System.Security.Claims;
using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    List<Deck> Deck_list { get; set; } = [];

    [Inject]
    private MatchService? _matchService { get; set; }

    [Inject]
    ICardsService? cardsService { get; set; }

    [Inject]
    private NavigationManager? navigation { get; set; }


    [Inject]
    private ClockService? _clockService { get; set; }

    bool TimerVisible { get; set; } = false;



    protected override async Task OnInitializedAsync()
    {

        await FetchUserDecks();
        if (_clockService is not null)
        {
            _clockService.Visibility += UpdateVisibility;
            TimerVisible = _clockService.Visible;
        }


        if (_matchService is null) { return; }

        _matchService.JoinGame += HandleJoinGame;

    }

    /* Fetch deck in the where state is done */
    private async Task FetchUserDecks()
    {
        if (cardsService is null) { return; }
        Deck_list = await cardsService.GetUserDeck("done");

        StateHasChanged();
    }

    public async void SearchGame()
    {
        if (_matchService is null || _matchService.Searching)
        { return; }
        await _matchService.SearchGameAsync();
        _clockService?.Start();
    }

    private async void UpdateVisibility(bool visible)
    {
        await InvokeAsync(() =>
        {
            TimerVisible = visible;
            StateHasChanged();
        });
    }

    private void HandleJoinGame(GameMatch gameMatch, Player player)
    {
        //navigation?.NavigateTo("/game");
        _clockService?.Stop();
        Console.WriteLine("Join Game");
    }

    public void Dispose()
    {
        if (_clockService is not null)
        {
            _clockService.Stop();
            _clockService.Visibility -= UpdateVisibility;
        }
    }
}