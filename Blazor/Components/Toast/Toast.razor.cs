
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Toast;

public partial class Toast : ComponentBase
{

  [Parameter, EditorRequired]
  public required CToast _toast { get; set; }

  [Inject]
  ToastService? _toast_service { get; set; }

  protected override async Task OnInitializedAsync()
  {
    if (_toast is null || _toast.Timeout == 0) { return; }

    await Task.Delay(_toast.Timeout);
    Remove();
  }

  private void Remove()
  {
    if (_toast_service is null) { return; }
    _toast_service.Remove(_toast);
    StateHasChanged();
  }

  public void Dispose()
  {

  }
}
