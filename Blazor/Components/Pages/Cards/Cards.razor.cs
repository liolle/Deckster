using Blazor.models;
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Cards;

public partial class Cards : ComponentBase
{
    List<Card> Card_list { get; set; } = [];

    [Inject]
    ICardsService? cardsService { get; set; }

    [Inject]
    NavigationManager? Navigation { get; set; }

    protected override void OnInitialized()
    {
        _ = UpdateCards();
    }


    private async Task UpdateCards()
    {
        if (cardsService is null) { return; }
        Card_list = await cardsService.GetAllCards();

        StateHasChanged();
    }

    private void NavigateToAdd()
    {
        Navigation?.NavigateTo("/cards/add");
    }

}
