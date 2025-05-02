
using deckster.cqs;
using deckster.dto;
using deckster.services;
using deckster.services.commands;
using deckster.extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using deckster.entities;
using System.Security.Claims;
using deckster.exceptions;
using deckster.services.queries;

namespace deckster.contollers;

public class DeckController(ICardService cards) : ControllerBase
{

  // Deck 
  [HttpPost]
  [Route("deck/add")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [Description("Create a deck")]
  public IActionResult AddDeck([FromBody] AddDeckCommand model)
  {

    try
    {
      this.validModelOrThrow();

      string account_id = User.FindFirst("AccountId")?.Value ?? "";
      DeckEntity deck = DeckEntity.Create(account_id, model.Name);
      model.AccountId = account_id;
      model.DeckId = deck.Id;

      CommandResult result = cards.Execute(model);

      if (!result.IsSuccess)
      {
        if (result.Exception is not null) { throw result.Exception; }
        return IApiOutput.Response(result.ErrorMessage);
      }

      DeckModel output = new(deck);

      return IApiOutput.Response(output);
    }
    catch (Exception e)
    {

      return IApiOutput.ResponseError(e);
    }
  }

  [HttpGet]
  [Route("decks/me")]
  [Authorize]
  [Description("Gets all user decks")]
  public IActionResult GetUserDecks()
  {
    try
    {
      string account_id = User.FindFirst("AccountId")?.Value ?? "";
      QueryResult<List<DeckEntity>> result = cards.Execute(new UserDecksQuery(account_id));

      if (!result.IsSuccess)
      {
        if (result.Exception is not null) { throw result.Exception; }
        return IApiOutput.Response(result.ErrorMessage);
      }

      return IApiOutput.Response(result.Result);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }

  [HttpDelete]
  [Route("deck")]
  [Authorize]
  [Description("Delete user decks")]
  public IActionResult DeleteCard([FromQuery] string deckId)
  {
    try
    {
      this.validModelOrThrow();
      string account_id = User.FindFirst("AccountId")?.Value ?? "";

      // TODO
      // replace with a service call

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {

      return IApiOutput.ResponseError(e);
    }
  }



  [HttpGet]
  [Route("deck/cards")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [Description("Gets deck with cars ")]
  public IActionResult GetDeckCard([FromQuery] string deckId)
  {
    try
    {
      this.validModelOrThrow();
      string account_id = User.FindFirst("AccountId")?.Value ?? "";

      // TODO
      // replace with a service call

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {

      return IApiOutput.ResponseError(e);
    }
  }

  [HttpPatch]
  [Route("deck/cards")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [Description("Gets deck with cars ")]
  public IActionResult UpdateDeckCard([FromBody] List<DeckModel> model)
  {
    try
    {
      foreach (DeckModel deckCard in model)
      {
        if (!TryValidateModel(deckCard))
        {
          throw new InvalidRequestModelException([]);
        }
      }

      string account_id = User.FindFirst("AccountId")?.Value ?? "";

      // TODO
      // replace with a service call

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {

      return IApiOutput.ResponseError(e);
    }
  }

}
