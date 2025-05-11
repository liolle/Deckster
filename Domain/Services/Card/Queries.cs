using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using deckster.cqs;
using deckster.dto;
using deckster.entities;
using Shared.models;

namespace deckster.services.queries;

public class CardsQuery(int page, int size) : IQueryDefinition<List<CardEntity>>
{
  [Range(0, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
  public int Page = page;

  [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50")]
  public int Size = size;
}

public class UserDecksQuery(string account_id) : IQueryDefinition<List<DeckEntity>>
{
  public string AccountId = account_id;
  public string State { get; set; } = "";
}

public class UserDecksInfoQuery(string deck_id) : IQueryDefinition<DeckModel>
{
  [JsonPropertyName("deckId")]
  public string DeckId = deck_id;
}

public class DeckCardsQuery(string deck_id) : IQueryDefinition<List<DeckCard>>
{
  public string DeckId = deck_id;
}