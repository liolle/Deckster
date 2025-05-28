namespace  Blazor.services.game.state;

public class GameInit : GameState
{
    private HashSet<string> _playerWaiting = [];
    public override async Task AfterInit()
    {
        await base.AfterInit();
        
        Console.WriteLine("Initializing game");
    }

    public override async Task<bool> PlayerReady(string playerId)
    {
        if (Context is null)
        {
            return false;
        }
        Console.WriteLine($"Ready to play {playerId}");
        await Task.CompletedTask;
        _playerWaiting.Add(playerId);

        if (_playerWaiting.Count == 2)
        {
            Context.TransitionTo(new GamePlayerPicking());
        }
        return true;
    }
    
}