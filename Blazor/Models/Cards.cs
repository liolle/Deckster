using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Blazor.Components.Pages.Cards.AddCard;

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

    public void Reset()
    {
        Name = "";
        Image = "default-image.png";
        Cost = 0;
        Defense = 0;
        Strength = 0;
    }

}

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

public class DeckInfo(Deck deck, List<DeckCard> cards)
{
    public Deck Deck { get; set; } = deck;

    public List<DeckCard> Cards { get; set; } = cards;

    public int Count
    {
        get
        {
            int output = 0;
            foreach (DeckCard item in Cards)
            {
                output += item.Quantity;
            }

            return output;
        }
    }

    public List<string> AddCard(DeckCard card)
    {
        List<string> errors = [];

        if (Count >= 30)
        {
            errors.Add("Max Deck card exceeded, Only 30 card per deck allowed");
        }

        if (CountById(card.CardId) >= 5)
        {
            errors.Add("Max Card type exceeded, Only 5 copy per card allowed");
        }

        if (errors.Count > 0)
        {
            return errors;
        }


        DeckCard? dc = Cards.Find(c => c.CardId == card.CardId);


        if (dc is null)
        {
            Cards.Add(card);
        }
        else
        {
            dc.Quantity++;
        }

        return errors;
    }

    public List<string> RemoveCard(string cardId)
    {
        List<string> errors = [];

        DeckCard? dc = Cards.Find(c => c.CardId == cardId);

        if (dc is null)
        {
            errors.Add("Can't remove card from deck");
            return errors;
        }

        dc.Quantity--;


        if (dc.Quantity < 1)
        {
            Cards.Remove(dc);
        }


        return errors;
    }

    public List<string> ValidateDeck()
    {
        List<string> errors = [];

        if (Count != 30)
        {
            errors.Add("Make sure to have 30 cards before saving");
        }

        return errors;
    }


    public PatchDeckModel GetPatchModel()
    {
        List<MinimalCard> cards = [];

        Cards.ForEach(c =>
        {
            cards.Add(new MinimalCard(c.CardId, c.Quantity));
        });

        PatchDeckModel model = new()
        {
            DeckId = Deck.Id,
            Cards = cards
        };

        return model;
    }

    public int CountById(string cardId)
    {
        return Cards.Find(c => c.CardId == cardId)?.Quantity ?? 0;
    }
}


public class Deck(string id, string name)
{
    public string Id { get; init; } = id;
    public string Name { get; init; } = name;

    public override bool Equals(object? obj)
    {
        if (obj is not Deck other)
        { return false; }

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

}

public class AddDeckModel
{
    [Required(ErrorMessage = "You need to specify a name for the deck")]
    [JsonPropertyName("name")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = "";


    public void Reset()
    {
        Name = "";
    }
}


public class MinimalCard(string cardId, int quantity)
{
    [JsonPropertyName("cardid")]
    public string CardId { get; set; } = cardId;
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = quantity;
}

public class PatchDeckModel
{
    [JsonPropertyName("deckid")]
    public string DeckId { get; set; } = "";
    [JsonPropertyName("cards")]
    public List<MinimalCard> Cards { get; set; } = [];

}