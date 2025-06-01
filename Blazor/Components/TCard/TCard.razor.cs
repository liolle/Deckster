using Blazor.models;
using Blazor.services;
using Microsoft.AspNetCore.Components;
namespace Blazor.Components.TCard;

public partial class TCard : ComponentBase
{
    [Inject]
    IConfiguration? Config { get; set; }

    [Parameter, EditorRequired]
    public Card CCard { get; set; } = null!;

    [Parameter]
    public int Quantity { get; set; } = 0;

    private string _fileServer = "";
    private string _defaultCard = "";
    private string? _image;

    protected override async Task OnInitializedAsync()
    {
        _fileServer = Config?["FILE_SERVER"] ?? throw new Exception("Missing configuration FILE_SERVER");
        _defaultCard = Config?["DEFAULT_CARD"] ?? throw new Exception("Missing configuration DEFAULT_CARD");
        await UpdateImage();
    }

    protected override async Task OnParametersSetAsync()
    {
        await UpdateImage();
        StateHasChanged();
    }

    private async Task UpdateImage()
    {
        _image = $"{_fileServer}/{CCard.Image}";

        bool exist = await ImageLinkChecker.ImageExistsAsync(_image);
        if (!exist)
        {
            _image = _image = $"{_fileServer}/{_defaultCard}";
        }
    }

    private string GetCount()
    {
        return $"x{Quantity}";
    }

    private string MaxedStatus()
    {
        return Quantity >= 5 ? "maxed" : "";
    }
}
