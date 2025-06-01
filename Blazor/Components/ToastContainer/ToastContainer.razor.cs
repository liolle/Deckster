
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.ToastContainer;

public partial class ToastContainer : ComponentBase
{
    public List<CToast> ToastList { get; set; } = [];

    [Inject]
    ToastService? Toast { get; set; }

    protected override void OnInitialized()
    {
        if (Toast is null) { return; }
        Toast.OnToastAdded += Add;
        Toast.OnToastRemoved += Remove;
        ToastList = Toast.ToastList;
    }

    private void Add(CToast toast)
    {
        StateHasChanged();
    }

    private void Remove(CToast toast)
    {
        StateHasChanged();
    }
   
    public void Dispose()
    {
        if (Toast is null) { return; }
        Toast.OnToastAdded -= Add;
        Toast.OnToastRemoved -= Remove;
    }
}