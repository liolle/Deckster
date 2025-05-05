namespace Blazor.models;

public record GameMatch(string player1, string player2)
{

}

public record Player
{

    public string Id { get; init; } = "";
    public string ConnectionId { get; init; } = "";
}

