namespace Blazor.services;

public enum TOAST_TYPE
{
    SUCCESS,
    INFO,
    ERROR,
    WARNING
}

public class CToast(TOAST_TYPE type, string content, int timeout)
{
    public TOAST_TYPE Type { get; } = type;
    public string Content { get; } = content;
    public int Timeout { get; } = timeout;
}

public class ToastService
{
    public List<CToast> ToastList { get; } = [];
    public event Action<CToast>? AddToast;
    public event Action<CToast>? RemoveToast;

    public void Add(CToast toast)
    {
        if (AddToast is null) { return; }
        ToastList.Add(toast);
        AddToast.Invoke(toast);
    }


    public void Remove(CToast toast)
    {
        if (RemoveToast is null) { return; }
        ToastList.Remove(toast);
        RemoveToast.Invoke(toast);
    }
}


