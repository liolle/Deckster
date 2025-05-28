namespace Blazor.models;

public class GameMatch
{
    public string Id { get; init; }
    
    public List<Player> Players { get; init; }

    public int NextToPlay { get; set; }  

    public GameMatch(List<Player> players)
    {
        Players = players;
        
        // Pick First to play
        Id = GenerateId();
        NextToPlay = -1;
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