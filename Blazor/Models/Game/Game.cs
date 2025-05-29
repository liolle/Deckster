using System.Text.Json.Serialization;
using Blazor.services;
using Blazor.services.game.state;

namespace Blazor.models;

public class GameMatch
{
    [JsonIgnore]
    public GameContext? Context { get; private set; }

    public string TurnTime
    {
        get
        {
            if (Context is null)
            {
                return "00:00";
                
            }

            return ClockService.FormatSecondsToHHMMSS(Context.GameClockService.Time);
        }
    }

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
    
    public void SetContext( GameContext context )
    {
        Context = context;
    }
}

public record Player
{
    [JsonIgnore]
    public PlayerConnectionContext? Context { get; private set; }

    public string Id { get; set; } = "";

    public string ConnectionId
    {
        get
        {
            if (Context is null)
            {
                return "";
            }
            return Context.ConnectionId;
        }
    } 

    public string NickName { get; set; } = "";

    public void SetContext( PlayerConnectionContext context )
    {
        Context = context;
    }
   
}