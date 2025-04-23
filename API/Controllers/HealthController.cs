
using Microsoft.AspNetCore.Mvc;

namespace deckster.contollers;

public class HealthController : ControllerBase
{
  [HttpGet]
  [Route("ping")]
  public IActionResult res()
  {
    return Ok("pong");
  }
}
