namespace Blazor.services;

public enum TOAST_TYPE
{
    SUCCESS,
    INFO,
    ERROR,
    WARNING
}

public class ToastService
{
    public event Action<TOAST_TYPE, string, int>? OnShow;
    public event Action? OnHide;

    public void ShowToast(TOAST_TYPE type, string message, int timeout = 3000)
    {
        if (OnShow is null) { return; }
        OnShow.Invoke(type, message, timeout);
    }


    public void HideToast()
    {
        OnHide?.Invoke();
    }

}