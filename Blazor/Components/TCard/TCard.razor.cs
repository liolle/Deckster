using Blazor.models;
using Blazor.services;
using Microsoft.AspNetCore.Components;
namespace Blazor.Components.TCard;

public partial class TCard : ComponentBase
{
    [Inject]
    IConfiguration? config { get; set; }

    [Parameter, EditorRequired]
    public Card CCard { get; set; } = null!;

    private string FILE_SERVER = "";
    private string DEFAULT_CARD = "";
    string? Image;

    protected override async Task OnInitializedAsync()
    {
        FILE_SERVER = config?["FILE_SERVER"] ?? throw new Exception("Missing configuration FILE_SERVER");
        DEFAULT_CARD = config?["DEFAULT_CARD"] ?? throw new Exception("Missing configuration DEFAULT_CARD");
        await UpdateImage();
    }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateImage();
        StateHasChanged();
    }

    private async Task UpdateImage()
    {
        Image = $"{FILE_SERVER}/{CCard.Image}";

        bool exist = await ImageLinkChecker.ImageExistsAsync(Image);
        if (!exist)
        {
            Image = Image = $"{FILE_SERVER}/{DEFAULT_CARD}";
        }
    }
}