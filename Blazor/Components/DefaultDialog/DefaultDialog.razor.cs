using Microsoft.AspNetCore.Components;

namespace Blazor.Components.DefaultDialog;

public partial class DefaultDialog : ComponentBase
{
    [Parameter] public bool CloseOnOverlayClick { get; set; } = true;

    [Parameter] public bool Visible { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    public void Close()
    {
        Visible = false;
        StateHasChanged();
    }

    public void Open()
    {
        Visible = true;
        StateHasChanged();
    }

    private void OverlayClose()
    {
        if (!CloseOnOverlayClick)
        { return; }
        Visible = false;
    }
}