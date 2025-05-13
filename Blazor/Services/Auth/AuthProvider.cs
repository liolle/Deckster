using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Blazor.services;

public class AuthProvider(IHttpContextAccessor httpContextAccessor, IConfiguration config) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            HttpContext? httpContext = httpContextAccessor.HttpContext ?? throw new Exception("Missing dependency HttpContextAccessor");
            string COOKIE_NAME = config["AUTH_TOKEN_NAME"] ?? throw new Exception("Missing configuration: AUTH_TOKEN_NAME");
            string accessToken = httpContext.Request.Cookies[COOKIE_NAME] ?? throw new Exception("Missing ACCESS_TOKEN");
            ClaimsPrincipal principal = ValidateToken(accessToken);
            await Task.CompletedTask;
            return new AuthenticationState(principal);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        string jwt_key = config["JWT_KEY"] ?? throw new Exception("Missing configuration JWT_KEY");
        TokenValidationParameters parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JWT_ISSUER"],
            ValidAudience = config["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key))
        };

        JwtSecurityTokenHandler handler = new();
        ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out SecurityToken validatedToken);

        return principal;
    }
}