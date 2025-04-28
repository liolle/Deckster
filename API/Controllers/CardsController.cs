using deckster.cqs;
using deckster.dto;
using deckster.services;
using deckster.services.commands;
using deckster.extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace deckster.contollers;

public class CardsController(ICardService cards) : ControllerBase 
{
  [HttpPost]
  [Route("card/add")]
  [EnableCors("AllowCredentials")]
  public IActionResult AddCard([FromBody] AddCardCommand command)
  {
    try
    {
      this.validModelOrThrow();

      CommandResult result = cards.Execute(command);

      if (!result.IsSuccess)
      {
        if (result.Exception is not null)
        {
          throw result.Exception;
        }

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
  [Route("cardById")]
  public IActionResult GetCard([FromQuery] string id)
  {
    return IApiOutput.Response(new{card_id = id});
  }

  [HttpGet]
  [Route("card")]
  public IActionResult GetAllCard()
  {
    return IApiOutput.Response(new List<string>());
  }

  [HttpPost]
  [Route("deck/add")]
  [EnableCors("AllowCredentials")]
  public IActionResult AddDeck()
  {
    return IApiOutput.Response(null);
  }


  [HttpPost]
  [Route("deck/card/add")]
  [EnableCors("AllowCredentials")]
  [Authorize]
  public IActionResult AddDeckCard()
  {
    return IApiOutput.Response(null);
  }
}
