namespace Blazor.services.game.state;

public class GamePlayerPlaying : GameState, IDisposable 
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
        if (Context is not null)
        {
            Context.GameClockService.Tick += HandleClockTick;
            
            BroadcastMessage("GamePlayerTurn", (p) =>
            {
                return [Context.Match.Players[Context.Match.NextToPlay].Id == p.Id];
            }); 
            Context.GameClockService.Start();
        }
        
    }

    private void HandleClockTick(int time)
    {
        if (Context is null)
        {
            return;
        }
        
        BroadcastMessage("GameTurnTick", (P) =>
        {
            return [Context.Match.Players[Context.Match.NextToPlay], time];
        }); 
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