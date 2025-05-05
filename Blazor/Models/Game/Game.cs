namespace Blazor.models;

public record GameMatch(string player1, string player2)
{

}

public record Player(string id, string connectionId)
{

    public string Id { get; init; } = id;
    public string ConnectionId { get; init; } = connectionId;
}

