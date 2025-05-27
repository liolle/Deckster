namespace Blazor.services;

public class GameClockService
{
    private int _baseTime;
    ClockState State { get; set; } = ClockState.stopped;
    public event Action<int>? Tick;
    
    private Timer? _timer;
    public int Time { get; private set; }


    public GameClockService(int seconds)
    {
        _baseTime = seconds;
    }
    
    public void Start()
    {
        if (State == ClockState.running)
        { return; }
        State = ClockState.running;
        _ = Launch();
    }

    public void Stop()
    {
        if (State == ClockState.stopped)
        { return; }
        State = ClockState.stopped;
        _timer?.Dispose();
    }

    public void Reset()
    {
        Time = _baseTime;
    }

    private void InvokeTick()
    {
        Time--;
        Tick?.Invoke(Time);
        if (Time <= 0)
        {
            Stop();
        }
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