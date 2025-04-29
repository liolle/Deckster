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
  [Authorize(Roles = "admin")]
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
  [Route("card")]
  public IActionResult GetAllCard([FromQuery] int page, int size)
  {
    return IApiOutput.Response(new List<string>());
  }
}
