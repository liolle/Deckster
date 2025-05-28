namespace Blazor.services.game.state;

public class GamePlayerPlaying : GameState, IDisposable 
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
        if (Context is not null)
        {
            Context.GameClockService.Tick += HandleClockTick;
            Context.GameClockService.Start();
            Console.WriteLine($"GamePlayerPlaying: [{Context.Match.Id}]\n- {Context.Match.NextToPlay}");
        }
        
    }

    private void HandleClockTick(int time)
    {
        Console.WriteLine($"Player: {Context?.Match.NextToPlay ?? -23} : {GameClockService.FormatSecondsToHHMMSS(time)} left");
        if (Context is not null && time <= 0)
        {
            EndTurn();
        }
    }

    public override async Task<bool> EndTurn(string playerId)
    {
        if (Context is null || !Context.State.IsPlayerTurn(playerId))
        {
            return false;
        }

        await Task.CompletedTask;
        return EndTurn();
    }

    private bool EndTurn()
    {
        if (Context is null)
        {
            return false;
        }
        Context.TransitionTo(new GamePlayerPicking()); 
        Dispose();
        return true;
    }

    public void Dispose()
    {
        if (Context is not null)
        {
            Context.GameClockService.Tick -= HandleClockTick;
        }
    }
}