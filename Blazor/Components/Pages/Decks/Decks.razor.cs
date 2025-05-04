
using Blazor.models;
using Blazor.services;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    private void SelectDeck(Deck? d)
    {
        if (d is null)
        {
            SelectedDeck = null;
        }
        else if (!d.Equals(SelectedDeck))
        {
            SelectedDeck = d;
        }
        _ = FetchDeckInfo();
        SelectCard(null);
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

        return SelectedDeckInfo.CountById(id);
    }

    private async Task SaveDeck()
    {
        if (SelectedDeckInfo is null || cardsService is null)
        { return; }
        await Task.CompletedTask;

        List<string> errs = SelectedDeckInfo.ValidateDeck();

        if (errs.Count > 0)
        {

            foreach (string item in errs)
            {
                _toast?.Add(new CToast(TOAST_TYPE.ERROR, item, 0));
            }

            return;
        }

        string error = await cardsService.PatchDeck(SelectedDeckInfo.GetPatchModel());

        if (!string.IsNullOrEmpty(error))
        {
            _toast?.Add(new CToast(TOAST_TYPE.ERROR, error, 0));
            return;

        }

        _toast?.Add(new CToast(TOAST_TYPE.SUCCESS, "Deck updated successfully", 4000));
    }

    private void HandleCardClick(MouseEventArgs e, Card c)
    {
        if (SelectedDeckInfo is null)
        {
            return;
        }

        List<string> errors = [];

        if (e.ShiftKey)
        {
            // remove one copy on SHIFT Click 
            SelectedDeckInfo.RemoveCard(c.Id).ForEach(err =>
            {
                errors.Add(err);
            });
        }
        else
        {
            // Add a copy on Click

            DeckCard new_card = new DeckCard(c.Id, c.Name, 1, c.Defense, c.Cost, c.Strength, c.Image);
            SelectedDeckInfo.AddCard(new_card).ForEach(err =>
            {
                errors.Add(err);
            }); ;
        }

        if (errors.Count == 0)
        {
            StateHasChanged();
        }
    }

    private async Task DeleteDeck()
    {
        if (SelectedDeck is null || cardsService is null)
        {
            return;
        }

        Deck_list.RemoveAll(d => d.Id == SelectedDeck.Id);
        string error = await cardsService.DeleteDeck(SelectedDeck.Id);
        SelectDeck(null);
        StateHasChanged();
    }
}