using Blazor.models;
using Blazor.services;
using Blazor.services.game;
using Blazor.services.game.state;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    private List<Deck> DeckList { get; set; } = [];

    [Inject]
    private MatchService? MatchService { get; set; }

    [Inject] private ICardsService? CardsService { get; set; }

    [Inject]
    private ClockService? ClockService { get; set; }

    bool FindGameVisible { get; set; } = false;


    protected override async Task OnInitializedAsync()
    {
        await FetchUserDecks();
        if (ClockService is not null)
        {
            ClockService.Visibility += UpdateVisibility;
            FindGameVisible = !ClockService.Visible;
        }
    }

    /* Fetch deck in the where state is done */
    private async Task FetchUserDecks()
    {
        if (CardsService is null) { return; }
        DeckList = await CardsService.GetUserDeck("done");

        StateHasChanged();
    }

    public async void SearchGame()
    {
        if (MatchService is null || MatchService.State != MatchState.lobby)
        { return; }
        await FetchUserDecks();
        if (DeckList.Count < 1)
        { return; }
        await MatchService.SearchGameAsync();
        ClockService?.Start();
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
        if (ClockService is not null)
        {
            ClockService.Visibility -= UpdateVisibility;
        }
    }
}