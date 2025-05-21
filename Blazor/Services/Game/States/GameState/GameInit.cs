namespace  Blazor.services.game.state;

public class GameInit : GameState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
        
        Console.WriteLine("Initializing game");
    }
    
    

}