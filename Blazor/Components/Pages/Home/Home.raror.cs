using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    List<Deck> Deck_list { get; set; } = [];

    [Inject]
    private MatchService? _matchService { get; set; }

    [Inject]
    ICardsService? cardsService { get; set; }

    [Inject]
    private ClockService? _clockService { get; set; }

    bool FindGameVisible { get; set; } = false;


    protected override async Task OnInitializedAsync()
    {
        await FetchUserDecks();
        if (_clockService is not null)
        {
            _clockService.Visibility += UpdateVisibility;
            FindGameVisible = !_clockService.Visible;
        }
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
        if (_matchService is null || _matchService.State != MatchState.lobby)
        { return; }
        await FetchUserDecks();
        if (Deck_list.Count < 1)
        { return; }
        await _matchService.SearchGameAsync();
        _clockService?.Start();
    }

    private async void UpdateVisibility(bool visible)
    {
        await InvokeAsync(() =>
        {
            FindGameVisible = !visible;
            StateHasChanged();
        });
    }



    public void Dispose()
    {
        if (_clockService is not null)
        {
            _clockService.Visibility -= UpdateVisibility;
        }
    }
}