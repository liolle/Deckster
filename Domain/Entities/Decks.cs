using System.Text.Json.Serialization;
using deckster.utils;

namespace deckster.entities;

public class DeckEntity(string id, string account_id, string name)
{
  public string Id { get; init; } = id;
  public string Account_id { get; init; } = account_id;
  public string Name { get; init; } = name;

  public static DeckEntity Create(string user_id, string name)
  {
    return new(IdGererator.GenerateId("DK"), user_id, name);
  }
}

public class DeckCardEntity(string deck_id, string card_id, int quatity)
{
  public string DeckId { get; init; } = deck_id;
  public string CardId { get; init; } = card_id;
  public int Quatity { get; init; } = quatity;

}


