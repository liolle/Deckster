namespace Blazor.services;

public enum ClockState
{
    stopped,
    running
}

public class ClockService
{
    ClockState State { get; set; } = ClockState.stopped;
    public event Action<int>? Tick;

    public event Action<bool>? Visibility;

    private Timer? _timer;

    public int Time { get; private set; } = 0;
    public bool Visible { get; private set; } = false;

    public void Start()
    {
        if (State == ClockState.running)
        { return; }
        State = ClockState.running;
        ShowClock(true);
        _ = Launch();
    }

    public void Stop()
    {
        if (State == ClockState.stopped)
        { return; }
        State = ClockState.stopped;
        ShowClock(false);
        _timer?.Dispose();
    }

    public void Reset()
    {
        Time = 0;
    }

    public void ShowClock(bool visible)
    {
        Visibility?.Invoke(visible);
        Visible = visible;
    }

    private void InvokeTick()
    {
        Time++;
        Tick?.Invoke(Time);
    }

    private async Task Launch()
    {
        _timer = new Timer(_ =>
        {
            if (State == ClockState.stopped)
            {
                _timer?.Dispose();
                return;
            }

            InvokeTick();

        }, null, 0, 1000);


        await Task.CompletedTask;
    }

    public static string FormatSecondsToHHMMSS(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

        if (totalSeconds <= 60 * 60)
        {
            return time.ToString(@"mm\:ss");
        }

        return time.ToString(@"hh\:mm\:ss");
    }
}