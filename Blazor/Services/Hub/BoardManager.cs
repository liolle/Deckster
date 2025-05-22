namespace Blazor.services;

public interface IBoardManager
{
    public Task PickPlayer();
}


public class BoardManager : IBoardManager
{
    
    public Task PickPlayer()
    {
        return Task.CompletedTask;
    }
}