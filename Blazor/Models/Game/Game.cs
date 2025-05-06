namespace Blazor.models;

public record GameMatch(Player player1, Player player2)
{

}

public record Player
{

    public string Id { get; init; } = "";
    public string ConnectionId { get; init; } = "";
    public string NickName { get; set; } = "";
}

