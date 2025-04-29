
using deckster.cqs;
using deckster.dto;
using deckster.services;
using deckster.services.commands;
using deckster.extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace deckster.contollers;

public class DeckController(ICardService cards) : ControllerBase 
{

  // Deck 
  [HttpPost]
  [Route("deck")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [Description("Create a deck")]
  public IActionResult AddDeck()
  {
    return IApiOutput.Response(null);
  }

  [HttpGet]
  [Route("deck")]
  [Authorize]
  [Description("Gets all user decks")]
  public IActionResult GetDeckCards([FromQuery] string deckId)
  {
    return IApiOutput.Response(new{deck_id = deckId});
  }

  [HttpDelete]
  [Route("deck")]
  [Authorize]
  [Description("Delete user decks")]
  public IActionResult GetAllCard([FromQuery] int page, int size)
  {
    return IApiOutput.Response(new List<string>());
  }


  // Deck cards
  [HttpPost]
  [Route("deck/card")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  public IActionResult AddDeckCard()
  {
    return IApiOutput.Response(null);
  }

  [HttpGet]
  [Route("deck/card")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  public IActionResult GetDeckCard()
  {
    return IApiOutput.Response(null);
  }

  [HttpDelete]
  [Route("deck/card")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  public IActionResult DeleteDeckCard()
  {
    return IApiOutput.Response(null);
  }
}
