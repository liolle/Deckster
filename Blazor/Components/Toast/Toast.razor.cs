
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Toast;

public partial class Toast : ComponentBase
{
  [Parameter, EditorRequired]
  public required CToast ctoast { get; set; }

  [Inject] 
  private ToastService? toastService { get; set; }

  protected override async Task OnInitializedAsync()
  {
    if (ctoast.Timeout == 0) { return; }

    await Task.Delay(ctoast.Timeout);
    Remove();
  }

  private void Remove()
  {
    if (toastService is null) { return; }
    toastService.Remove(ctoast);
    StateHasChanged();
  }
}
