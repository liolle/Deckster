
using deckster.dto;
using Microsoft.AspNetCore.Mvc;

namespace deckster.contollers;

public class HealthController : ControllerBase
{
  [HttpGet]
  [Route("ping")]
  public IActionResult  Pong()
  {
    return IApiOutput.Reponse("Pong");
  }
}
