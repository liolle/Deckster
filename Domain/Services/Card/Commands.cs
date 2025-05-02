using System.ComponentModel.DataAnnotations;
using deckster.cqs;
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

