using System.Text.Json.Serialization;
using deckster.utils;

namespace deckster.entities;

public class DeckEntity(string id, string account_id, string name)
{
  public string Id { get; init; } = id;
  [JsonIgnore]
  public string Account_id { get; init; } = account_id;
  public string Name { get; init; } = name;

  [JsonPropertyName("state")]
  public string State { get; set; } = "";

  public static DeckEntity Create(string user_id, string name)
  {
    return new(IdGererator.GenerateId("DK"), user_id, name);
  }

  public override string ToString()
  {
    return $"=>{Id}: {Account_id}: {Name}";
  }
}

public class DeckCardEntity(string cardId, int quantity, string? deckId)
{
  [JsonPropertyName("deckid")]
  [JsonIgnore]
  public string DeckId { get; init; } = deckId ?? "";
  [JsonPropertyName("cardid")]
  public string CardId { get; init; } = cardId;
  [JsonPropertyName("quantity")]
  public int Quantity { get; init; } = quantity;

}


