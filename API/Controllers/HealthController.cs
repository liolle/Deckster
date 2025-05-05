
using deckster.dto;
using Microsoft.AspNetCore.Mvc;

namespace deckster.controllers;

public class HealthController : ControllerBase
{
  [HttpGet]
  [Route("ping")]
  public IActionResult Pong()
  {
    return IApiOutput.Response("Pong");
  }
}
