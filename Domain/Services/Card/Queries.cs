using System.ComponentModel.DataAnnotations;
using deckster.cqs;
using deckster.entities;

namespace deckster.services.queries;

public class CardsQuery(int page, int size) : IQueryDefinition<List<CardEntity>>
{
  [Range(0, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
  public int Page = page; 

  [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50")]
  public int Size = size; 
}
