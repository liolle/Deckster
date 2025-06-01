namespace Blazor.services;

public enum ToastType
{
    SUCCESS,
    INFO,
    ERROR,
    WARNING
}

public class CToast(ToastType type, string content, int timeout)
{
    public ToastType Type { get; } = type;
    public string Content { get; } = content;
    public int Timeout { get; } = timeout;
}

public class ToastService
{
    public List<CToast> ToastList { get; } = [];
    public event Action<CToast>? OnToastAdded;
    public event Action<CToast>? OnToastRemoved;

    public void Add(CToast toast)
    {
        if (OnToastAdded is null) { return; }
        ToastList.Add(toast);
        OnToastAdded.Invoke(toast);
    }

    public void Remove(CToast toast)
    {
        if (OnToastRemoved is null) { return; }
        ToastList.Remove(toast);
        OnToastRemoved.Invoke(toast);
    }
}