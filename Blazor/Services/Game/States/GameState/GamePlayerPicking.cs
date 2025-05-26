namespace Blazor.services.game.state;

public class GamePlayerPicking : GameState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
        
       if(Context is null) { return; }

       if (Context.Match.NextToPlay is null)
       {
           Context.Match.NextToPlay = 0;
           return;
       }
       
       Context.Match.NextToPlay += 1;
       Context.Match.NextToPlay %= 2;
    }
}