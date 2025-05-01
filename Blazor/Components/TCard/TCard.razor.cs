using Blazor.models;
using Microsoft.AspNetCore.Components;
namespace Blazor.Components.TCard;

public partial class TCard : ComponentBase
{
    [Inject]
    IConfiguration? config { get; set; }

    [Parameter, EditorRequired]
    public Card CCard { get; set; } = null!;
    string? Image;

    protected override void OnInitialized()
    {
        string FILE_SERVER = config?["FILE_SERVER"] ?? "";
        if (string.IsNullOrEmpty(FILE_SERVER) || CCard is null) { return; }

        Image = $"{FILE_SERVER}/{CCard.Image}";
    }
}