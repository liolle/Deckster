using Microsoft.AspNetCore.Mvc;
using deckster.dto;
using deckster.services.commands;
using deckster.exceptions;
using deckster.services;
using deckster.cqs;
namespace deckster.contollers;

public class AuthController(IAuthService auth) : ControllerBase 
{

  [HttpPost]
  [Route("register")]
  public IActionResult Register([FromBody] RegisterUserCommand command)
  {
    try
    {
      // Check Model 
      if (!ModelState.IsValid){
        var errors = ModelState
          .Where(x => x.Value?.Errors.Count > 0)
          .ToDictionary(
              val => val.Key,
              val => val.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
              ).ToArray();
        throw new InvalidRequestModelException(errors);
      }

      CommandResult result =  auth.Execute(command);

      if (!result.IsSuccess )
      {
        if (result.Exception is not null )
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
}

