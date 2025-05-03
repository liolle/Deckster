
using Blazor.models;
using Blazor.services;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Components;
using DefaultD = Blazor.Components.DefaultDialog.DefaultDialog;


namespace Blazor.Components.Pages.Decks;

[RequireCSRF]
public partial class Decks : ComponentBase
{
    private bool SubmittingDeck { get; set; } = false;

    List<Card> Card_list { get; set; } = [];

    public AddDeckModel Model { get; set; } = new();

    private DefaultD? addDeckDialog;

    private DeckCard? SelectedCard { get; set; } = null;

    List<Deck> Deck_list { get; set; } = [];

    DeckInfo? SelectedDeckInfo { get; set; } = null;


    Deck? SelectedDeck = null;

    [Inject]
    ICardsService? cardsService { get; set; }

    [Inject]
    ToastService? _toast { get; set; }

    private string SelectedTab { get; set; } = "decks";

    protected override void OnInitialized()
    {
        _ = FetchUserDecks();
        _ = FetchCards();
    }

    private string ShowSelected(string tab)
    {
        return SelectedTab == tab ? "tab-selected" : "";
    }

    private string ShowSelectedDeck(Deck d)
    {
        return d.Equals(SelectedDeck) ? "deck-selected" : "";
    }

    private string ShowSelectedCard(DeckCard d)
    {
        return d.Equals(SelectedCard) ? "card-selected" : "";
    }

    private void SelectDeck()
    {
        SelectedTab = "decks";

    }

    private void SelectCards()
    {
        SelectedTab = "cards";
    }

    private void SelectDeck(Deck d)
    {
        if (!d.Equals(SelectedDeck))
        { SelectCard(null); }
        SelectedDeck = d;
        _ = FetchDeckInfo();
        StateHasChanged();
    }

    private void SelectCard(DeckCard? c)
    {
        SelectedCard = c;
        StateHasChanged();
    }

    private async Task FetchUserDecks()
    {
        if (cardsService is null) { return; }
        Deck_list = await cardsService.GetUserDeck();

        StateHasChanged();
    }

    private async Task FetchCards()
    {
        if (cardsService is null) { return; }
        Card_list = await cardsService.GetAllCards();

        StateHasChanged();
    }

    private async Task FetchDeckInfo()
    {
        if (cardsService is null || SelectedDeck is null) { return; }
        SelectedDeckInfo = await cardsService.GetDeckInfo(SelectedDeck.Id);

        StateHasChanged();
    }

    private void OpenAddDeckDialog()
    {
        addDeckDialog?.Open();
    }

    private void CloseAddDeckDialog()
    {
        addDeckDialog?.Close();
    }

    private async Task SubmitNewDeck()
    {
        if (SubmittingDeck || cardsService is null)
        { return; }
        SubmittingDeck = true;
        CloseAddDeckDialog();

        string error = await cardsService.AddDeck(Model);

        if (!string.IsNullOrEmpty(error))
        {
            _toast?.Add(new CToast(TOAST_TYPE.ERROR, error, 0));
            SubmittingDeck = false;
            return;
        }
        _toast?.Add(new CToast(TOAST_TYPE.SUCCESS, "Deck add successfully", 4000));
        Model.Reset();

        await FetchUserDecks();
        SubmittingDeck = false;
    }
    private string MaxedStatus()
    {
        return (SelectedDeckInfo?.Count ?? 0) == 30 ? "maxed" : "";
    }

    private int GetCardCount(string id)
    {
        if (SelectedDeckInfo is null)
        { return 0; }

        return SelectedDeckInfo.Cards.Find(c => c.CardId == id)?.Quantity ?? 0;
    }

    private async Task SaveDeck()
    {
        await Task.CompletedTask;
        _toast?.Add(new CToast(TOAST_TYPE.INFO, "Test", 0));

    }
}