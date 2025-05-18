namespace Blazor.models;

public class GameMatch
{
    public Player Player1 { get; } 
    public Player Player2 { get; }
   
    public Player NexToPlay { get;  }

    public GameMatch(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
        
        // Pick First to play
        NexToPlay = player1;
    }
    
    
    
}

public record Player
{

    public string Id { get; init; } = "";
    public string ConnectionId { get; init; } = "";
    public string NickName { get; set; } = "";
}

