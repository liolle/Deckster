using System.Text.Json.Serialization;
using deckster.utils;

namespace deckster.entities;

public class CardEntity(string id, string name, int cost, int defense, int strength, string image)
{
  [JsonPropertyName("id")]
  public string Id { get; init; } = id;
  [JsonPropertyName("name")]
  public string Name { get; init; } = name;
  [JsonPropertyName("defense")]
  public int Defense { get; init; } = defense;
  [JsonPropertyName("cost")]
  public int Cost { get; init; } = cost;
  [JsonPropertyName("strength")]
  public int Strength { get; init; } = strength;
  [JsonPropertyName("image")]
  public string Image { get; init; } = image;

  public static CardEntity Create(string name, int cost, int defense, int strenght, string image)
  {
    return new(IdGererator.GenerateId("CRD"), name, cost, defense, strenght, image);
  }
}

