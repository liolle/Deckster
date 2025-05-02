using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using deckster.cqs;
using deckster.entities;
namespace deckster.services.commands;


public class AddCardCommand(string name, int cost, int defense, int strength, string image) : ICommandDefinition
{
  [Required(ErrorMessage = "Card name is required")]
  public string Name { get; set; } = name;

  public int Cost { get; set; } = cost;
  public int Defense { get; set; } = defense;
  public int Strength { get; set; } = strength;

  [Required(ErrorMessage = "Card image is required")]
  public string Image { get; set; } = image;
}

public class AddDeckCommand(string name) : ICommandDefinition
{
  [Required(ErrorMessage = "Deck name is required")]
  public string Name { get; set; } = name;
  public string AccountId { get; set; } = "";
  public string DeckId { get; set; } = "";
}



public class DeleteDeckCommand(string deckId) : ICommandDefinition
{
  [Required(ErrorMessage = "Deck id is required")]
  public string DeckId { get; set; } = deckId;

  [JsonIgnore]
  public string AccountId { get; set; } = "";

}

public class DeleteDeckCardsCommand(string deckId) : ICommandDefinition
{
  [Required(ErrorMessage = "Deck id is required")]
  public string DeckId { get; set; } = deckId;
}

public class PatchDeckCommand(string deckId, List<DeckCardEntity> cards) : ICommandDefinition
{
  [Required(ErrorMessage = "Deck id is required")]
  [JsonPropertyName("deckid")]
  public string DeckId { get; set; } = deckId;

  [JsonIgnore]
  public string AccountId { get; set; } = "";

  [JsonPropertyName("cards")]
  public List<DeckCardEntity> Cards { get; set; } = cards;

  public List<string> Validate()
  {
    int cnt = 0;
    List<string> errors = [];
    foreach (DeckCardEntity c in Cards)
    {
      cnt += c.Quantity;
    }


    if (cnt != 30) { errors.Add($"Invalid card number in the deck Expected:{30} Found: {cnt}"); }

    return errors;
  }
}

public class GetDeckPermission(string deck_id, string account_id) : ICommandDefinition
{
  public string DeckId { get; set; } = deck_id;
  public string AccountId { get; set; } = account_id;
}
