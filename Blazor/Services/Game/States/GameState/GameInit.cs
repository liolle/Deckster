namespace  Blazor.services.game.state;

public class GameInit : GameState
{
    private HashSet<string> _playerWaiting = [];
    public override async Task AfterInit()
    {
        await base.AfterInit();
        if (Context is not null)
        {
            _playerWaiting.Add(Context.Match.Players[0].Id);
            _playerWaiting.Add(Context.Match.Players[1].Id);
        }
        
        Console.WriteLine("Initializing game");
    }

    public override async Task<bool> PlayerReady(string playerId)
    {
        Console.WriteLine($"Ready to play {playerId}");
        if (Context is null)
        {
            return false;
        }
        
        await Task.CompletedTask;
        _playerWaiting.Remove(playerId);

        if (_playerWaiting.Count == 0)
        {
            Context.TransitionTo(new GamePlayerPicking());
        }
        return true;
    }
    
}