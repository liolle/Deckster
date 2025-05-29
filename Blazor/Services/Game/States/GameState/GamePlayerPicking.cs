namespace Blazor.services.game.state;

public class GamePlayerPicking : GameState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();

        if (Context is null)
        {
            return;
        }
       
        Context.Match.NextToPlay = (Context.Match.NextToPlay + 1)%2;
     
        Context.GameClockService.Stop();
        Context.GameClockService.Reset();
        Context.TransitionTo(new GamePlayerPlaying());
    }
}