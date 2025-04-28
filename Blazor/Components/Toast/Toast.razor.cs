
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Toast;

public partial class Toast : ComponentBase
{
    private bool Visible { get; set; } = false;

    private string Type { get; set; } = Enum.GetName(TOAST_TYPE.INFO)!;
    private string Content { get; set; } = "";
    private int Timeout { get; set; } = 0;

    [Inject]
    ToastService? _toast {get;set;}

    protected override void OnInitialized()
    {
      if (_toast is null){return;}
      _toast.OnShow += Show;
      _toast.OnHide += Hide;
    }

    private async void Show(TOAST_TYPE type, string content, int timeout)
    {
      Type = Enum.GetName(type)!;
      Content = content;
      Visible = true;
      Timeout = timeout;

      StateHasChanged();
      if (timeout <= 0) { return; }

      await Task.Delay(timeout);
      Hide();
    }

    private void Hide()
    {
      Visible = false;
      StateHasChanged();
    }

    public void Dispose()
    {
      if (_toast is null){return;}
      _toast.OnShow -= Show;
      _toast.OnHide -= Hide;
    }
}
