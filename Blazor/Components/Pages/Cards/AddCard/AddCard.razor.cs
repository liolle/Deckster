using Blazor.models;
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Cards.AddCard;

public partial class AddCard : ComponentBase
{

    public AddCardModel Model { get; set; } = new();

    [Inject]
    private ICardsService? _cards { get; set; }

    [Inject]
    private ToastService? _toast { get; set; }

    private bool Sending = false;


    private async Task SubmitValidFrom()
    {
        if (_cards is null) { return; }
        Sending = true;

        string error_msg = await _cards.AddCard(Model);

        if (!string.IsNullOrEmpty(error_msg))
        {
            _toast?.Add(new CToast(TOAST_TYPE.ERROR, error_msg, 5000));
            Sending = false;
            return;
        }
        _toast?.Add(new CToast(TOAST_TYPE.SUCCESS, "Card Added successfully", 3000));
        Sending = false;
    }
}