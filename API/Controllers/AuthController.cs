using Microsoft.AspNetCore.Mvc;
using deckster.dto;
using deckster.services.commands;
using deckster.exceptions;
using deckster.services;
using deckster.cqs;
using deckster.services.queries;
using deckster.extensions;
using Microsoft.AspNetCore.Cors;
namespace deckster.contollers;

public class AuthController(IAuthService auth, IConfiguration configuration) : ControllerBase
{

  [HttpPost]
  [Route("register")]
  [EnableCors("AllowCredentials")]
  public IActionResult Register([FromBody] RegisterUserCommand command)
  {
    try
    {
      this.validModelOrThrow();
      CommandResult result = auth.Execute(command);

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

  [HttpPost]
  [Route("login")]
  [EnableCors("AllowCredentials")]
  public IActionResult Login([FromBody] CredentialLoginQuery query)
  {
    try
    {
      this.validModelOrThrow();

      string token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
      string domain = configuration["DOMAIN"] ?? throw new MissingConfigException("DOMAIN");
      string expiry = configuration["JWT_EXPIRY"] ?? throw new MissingConfigException("JWT_EXPIRY");

      QueryResult<string> result = auth.Execute(query);

      if (!result.IsSuccess)
      {
        if (result.Exception is not null)
        {
          throw result.Exception;
        }

        return IApiOutput.Response(result.ErrorMessage);
      }

      CookieOptions cookieOptions = new()
      {
        HttpOnly = true,
        Domain = $"{domain}",
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddMinutes(60)
      };

      Response.Cookies.Append(token_name, result.Result, cookieOptions);

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }

  [HttpGet]
  [Route("logout")]
  [EnableCors("AllowCredentials")]
  public IActionResult Logout()
  {
    try
    {
      string token_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
      string domain = configuration["DOMAIN"] ?? throw new MissingConfigException("DOMAIN");

      CookieOptions cookieOptions = new()
      {
        HttpOnly = true,
        Domain = $"{domain}",
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.Now.AddDays(-1)
      };

      Response.Cookies.Append(token_name, string.Empty, cookieOptions);

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }
}

