using Blazor.services;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.FindMatchClock;

public partial class FindMatchClock : ComponentBase, IDisposable
{

    [Inject]
    ClockService? _clockService { get; set; }

    [Inject]
    private MatchService? _matchService { get; set; }

    string Content { get; set; } = "00:00";
    bool Visible { get; set; } = false;

    protected override void OnInitialized()
    {
        if (_clockService is null) { return; }
        _clockService.Tick += Tick;
        _clockService.Visibility += ShowClock;

        Content = ClockService.FormatSecondsToHHMMSS(_clockService.Time);
        Visible = _clockService.Visible;
    }

    private async void Tick(int time)
    {
        await InvokeAsync(() =>
        {
            Content = ClockService.FormatSecondsToHHMMSS(time);
            StateHasChanged();
        });
    }

    private async void ShowClock(bool visible)
    {
        await InvokeAsync(() =>
        {
            Visible = visible;
            StateHasChanged();
        });
    }

    private void OnTimerClicked()
    {
        _clockService?.Reset();
        _clockService?.Stop();
        _matchService?.LeaveGameAsync();
    }

    public void Dispose()
    {
        if (_clockService is null) { return; }
        _clockService.Stop();
        _clockService.Tick -= Tick;
        _clockService.Visibility += ShowClock;
    }
}