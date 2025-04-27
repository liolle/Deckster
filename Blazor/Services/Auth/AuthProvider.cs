using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.models;
using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.services;

public class AuthProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        string? accessToken = httpContext.Request.Cookies["disney_battle_auth_token"];

        if (accessToken is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jsonToken = handler.ReadJwtToken(accessToken);

        string? email = jsonToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        string? nickname = jsonToken.Claims.FirstOrDefault(c => c.Type == "Nickname")?.Value;
        string? id = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        string? provider = jsonToken.Claims.FirstOrDefault(c => c.Type == "Provider")?.Value;

        if (email is null || id is null || provider is null || nickname is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        List<Claim> claims =
          [
          new (nameof(User.Id), id),
          new (nameof(User.Email), email),
          new (nameof(User.Nickname), nickname),
          new ("Provider", provider),
          ];

        ClaimsIdentity identity = new(claims, "cookieAuth");

        await Task.CompletedTask;
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

}