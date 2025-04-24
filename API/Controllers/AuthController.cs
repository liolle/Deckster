using Microsoft.AspNetCore.Mvc;
using deckster.dto;
using deckster.services.commands;
using deckster.exceptions;
namespace deckster.contollers;

public class AuthController : ControllerBase 
{

  [HttpPost]
  [Route("register")]
  public IActionResult Register([FromBody] RegisterUserCommand command)
  {
    try
    {
      if (!ModelState.IsValid){
        // TODO 
        // Create la list of invalid field with the corresponding error message
        throw new InvalidRequestModelException();
      }

      // TODO
      // Use The Auth service

      return IApiOutput.Reponse(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e); 
    }
  }
}

