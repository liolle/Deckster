
using Blazor.services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.ToastContainer;

public partial class ToastContainer : ComponentBase
{
    public List<CToast> ToastList { get; set; } = [];

    [Inject]
    ToastService? _toast { get; set; }

    protected override void OnInitialized()
    {
        if (_toast is null) { return; }
        _toast.AddToast += Add;
        _toast.RemoveToast += Remove;
        ToastList = _toast.ToastList;
    }

    private void Add(CToast toast)
    {
        StateHasChanged();
    }

    private void Remove(CToast toast)
    {
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {

    }

    public void Dispose()
    {
        if (_toast is null) { return; }
        _toast.AddToast -= Add;
        _toast.RemoveToast -= Remove;
    }
}