namespace Blazor.services.game.state;

public class GamePlayerPicking : GameState
{
    public override async Task AfterInit()
    {
        await base.AfterInit();
        Console.WriteLine($"Game: {Context?.Match.Id}\n [Player picking]\n");
    }
}