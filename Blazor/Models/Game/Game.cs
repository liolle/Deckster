namespace Blazor.models;

public class GameMatch
{
    public string Id { get; init; }
    
    public List<Player> Players { get; init; }

    public int? NextToPlay { get; set; }  

    public GameMatch(Player player1, Player player2)
    {
        Players = [
             player1,
             player2
        ];
        
        // Pick First to play
        Id = GenerateId();
    }

    private string GenerateId()
    {
        string gui = Guid.NewGuid().ToString().Replace("-","");
        return $"GAME{gui}";
    }
}

public record Player
{
    public string Id { get; init; } = "";
    public string ConnectionId { get; init; } = "";
    public string NickName { get; set; } = "";
}