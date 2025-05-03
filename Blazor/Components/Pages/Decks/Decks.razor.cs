
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

    public AddDeckModel Model { get; set; } = new();

    private DefaultD? addDeckDialog;

    List<Deck> Deck_list { get; set; } = [];

    Deck? SelectedDeck = null;

    [Inject]
    ICardsService? cardsService { get; set; }

    [Inject]
    ToastService? _toast { get; set; }

    private string SelectedTab { get; set; } = "decks";

    protected override void OnInitialized()
    {
        _ = FetchUserDecks();
    }

    private string ShowSelected(string tab)
    {
        return SelectedTab == tab ? "tab-selected" : "";
    }

    private string ShowSelectedDeck(Deck d)
    {
        return d.Equals(SelectedDeck) ? "deck-selected" : "";
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
        SelectedDeck = d;
        StateHasChanged();
    }

    private async Task FetchUserDecks()
    {
        if (cardsService is null) { return; }
        Deck_list = await cardsService.GetUserDeck();

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
}