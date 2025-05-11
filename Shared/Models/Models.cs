using System.Text.Json.Serialization;

namespace Shared.models;

public class DeckCard(string cardId, string name, int quantity, int defense, int cost, int strength, string image)
{
    public string CardId { get; set; } = cardId;
    public int Quantity { get; set; } = quantity;
    public string Name { get; set; } = name;
    public int Defense { get; set; } = defense;
    public int Cost { get; set; } = cost;
    public int Strength { get; set; } = strength;
    public string Image { get; set; } = image;

    public override bool Equals(object? obj)
    {
        if (obj is not DeckCard other)
        { return false; }

        return CardId == other.CardId;
    }

    public override int GetHashCode()
    {
        return CardId.GetHashCode();
    }
}