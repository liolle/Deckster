
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Decks;


public partial class Decks : ComponentBase
{

    private string SelectedTab { get; set; } = "decks";

    private string ShowSelected(string tab)
    {
        return SelectedTab == tab ? "tab-selected" : "";
    }

    private void SelectDeck()
    {
        SelectedTab = "decks";
    }

    private void SelectCards()
    {
        SelectedTab = "cards";
    }
}