using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Blazor.models;
public class Card(string id, string name, int cost, int defense, int strength, string image)
{
    public string Id { get; init; } = id;
    public string Name { get; init; } = name;
    public int Defense { get; init; } = defense;
    public int Cost { get; init; } = cost;
    public int Strength { get; init; } = strength;
    public string Image { get; init; } = image;


    public override string ToString()
    {
        return $"Name: {Name}\n - Cost: {Cost}\n - Strength: {Strength}\n - Defense: {Defense}\nImage: {Image}";
    }
}

public class AddCardModel
{
    [Required(ErrorMessage = "Card name is required")]
    [JsonPropertyName("name")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Explicitly select 0 if the card as a Cost of 0")]
    [Range(0, 100, ErrorMessage = "Cost should be in the between 0-100 included")]
    [JsonPropertyName("cost")]
    public int Cost { get; set; }

    [Required(ErrorMessage = "Explicitly select 0 if the card as a Defense of 0")]
    [Range(0, 100, ErrorMessage = "Defense should be in the between 0-100 included")]
    [JsonPropertyName("defense")]
    public int Defense { get; set; }

    [Required(ErrorMessage = "Explicitly select 0 if the card as a Strength of 0")]
    [Range(0, 100, ErrorMessage = "Strength should be in the between 0-100 included")]
    [JsonPropertyName("strength")]
    public int Strength { get; set; }

    [Required(ErrorMessage = "You need to specify a file name as image\n this file should be present cards directory in the file server")]
    [RegularExpression(@".*\.png", ErrorMessage = "Only .png images are supported now")]
    [JsonPropertyName("image")]
    public string Image { get; set; } = "default-image.png";

    public Card GetCard()
    {
        return new Card("", Name, Cost, Defense, Strength, Image);
    }
}