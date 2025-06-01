using Microsoft.AspNetCore.Mvc;
using deckster.dto;
using deckster.services.commands;
using deckster.services;
using deckster.cqs;
using deckster.services.queries;
using deckster.extensions;
using Microsoft.AspNetCore.Cors;
using Shared.exceptions;
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

      string tokenName = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
      string domain = configuration["DOMAIN"] ?? throw new MissingConfigException("DOMAIN");
      string expiryStr = configuration["JWT_EXPIRY"] ?? throw new MissingConfigException("JWT_EXPIRY");
      
      if (!double.TryParse(expiryStr, out double expiry))
      {
        throw new MalformedConfiguration("JWT_EXPIRY");
      }

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
        Expires = DateTime.UtcNow.AddMinutes(expiry)
      };

      Response.Cookies.Append(tokenName, result.Result, cookieOptions);

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
      string tokenName = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
      string domain = configuration["DOMAIN"] ?? throw new MissingConfigException("DOMAIN");

      CookieOptions cookieOptions = new()
      {
        HttpOnly = true,
        Domain = $"{domain}",
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.Now.AddDays(-1)
      };

      Response.Cookies.Append(tokenName, string.Empty, cookieOptions);

      return IApiOutput.Response(null);
    }
    catch (Exception e)
    {
      return IApiOutput.ResponseError(e);
    }
  }

  [HttpPost]
  [Route("admin/promote")]
  [EnableCors("AllowCredentials")]
  public IActionResult PromoteAdmin([FromBody] PromoteAdminCommand command)
  {
    try
    {
      this.validModelOrThrow();
      this.validSuperAdminOrThrow(configuration, Request.Headers["X-ADMIN"]);

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
}

