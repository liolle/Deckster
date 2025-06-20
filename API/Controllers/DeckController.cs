
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
using deckster.services.queries;
using edllx.dotnet.csrf;
using Shared.exceptions;

namespace deckster.contollers;

public class DeckController(ICardService cards) : ControllerBase
{
  // Deck 
  [HttpPost]
  [Route("deck/add")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [RequireCSRF]
  [Description("Create a deck")]
  public IActionResult AddDeck([FromBody] AddDeckCommand model)
  {
    try
    {
      this.validModelOrThrow();

      string accountId = User.FindFirst("AccountId")?.Value ?? "";
      DeckEntity deck = DeckEntity.Create(accountId, model.Name);
      model.AccountId = accountId;
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
  public IActionResult GetUserDecks([FromQuery] string? state)
  {
    try
    {
      string accountId = User.FindFirst("AccountId")?.Value ?? "";

      UserDecksQuery q = new(accountId)
      {
        State = state ?? ""
      };

      QueryResult<List<DeckEntity>> result = cards.Execute(q);

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

  [HttpDelete("deck/{deckId}")]
  [Authorize]
  [RequireCSRF]
  [Description("Delete user decks")]
  public IActionResult DeleteCard(string deckId)
  {
    try
    {
      DeleteDeckCommand command = new(deckId);
      string accountId = User.FindFirst("AccountId")?.Value ?? "";
      command.AccountId = accountId;

      CommandResult result = cards.Execute(command);

      if (!result.IsSuccess)
      {
        if (result.Exception is not null) { throw result.Exception; }
        return IApiOutput.Response(result.ErrorMessage);
      }

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }



  [HttpGet]
  [Route("deck/cards")]
  [Description("Gets deck with cars ")]
  public IActionResult GetDeckCard([FromQuery] string deckId)
  {
    try
    {
      this.validModelOrThrow();

      QueryResult<DeckModel> result = cards.Execute(new UserDecksInfoQuery(deckId));

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

  [HttpPatch]
  [Route("deck/cards")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  [RequireCSRF]
  [Description("Patch deck ")]
  public IActionResult UpdateDeckCard([FromBody] PatchDeckCommand model)
  {
    try
    {
      this.validModelOrThrow();
      List<string> errors = model.Validate();

      if (errors.Count > 0)
      {
        throw new InvalidRequestModelException<List<string>>(errors);
      }

      string accountId = User.FindFirst("AccountId")?.Value ?? "";
      model.AccountId = accountId;
      CommandResult permissionResult = cards.Execute(new GetDeckPermission(model.DeckId, accountId));
      CommandResult res = cards.Execute(new DeleteDeckCardsCommand(model.DeckId));

      if (!permissionResult.IsSuccess)
      {
        if (permissionResult.Exception is not null) { throw permissionResult.Exception; }
        return IApiOutput.Response(permissionResult.ErrorMessage);
      }

      CommandResult result = cards.Execute(model);
      if (!result.IsSuccess)
      {
        if (result.Exception is not null) { throw result.Exception; }
        return IApiOutput.Response(result.ErrorMessage);
      }

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }
}
