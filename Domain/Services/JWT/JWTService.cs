using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

using deckster.exceptions;

namespace deckster.services;

public interface IJWTService
{
  public string Generate(List<Claim> claims);
}

public class JwtService : IJWTService
{
  private readonly string _secretKey;
  private readonly string _issuer;
  private readonly string _audience;

  public JwtService(IConfiguration configuration)
  {
    string? jwt_key = configuration["JWT_KEY"] ?? throw new MissingConfigException("JWT_KEY");
    string? jwt_issuer = configuration["JWT_ISSUER"] ?? throw new MissingConfigException("JWT_ISSUER");
    string? jwt_audience = configuration["JWT_AUDIENCE"] ?? throw new MissingConfigException("JWT_AUDIENCE");

    _secretKey = jwt_key;
    _issuer = jwt_issuer;
    _audience = jwt_audience;
  }

  public string Generate(List<Claim> claims)
  {

    // Standardized User info
    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    ClaimsPrincipal principal = new(identity);

    // Token signing info
    SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
    SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

    JwtSecurityToken token = new(
        issuer: _issuer,
        audience: _audience,
        claims: principal.Claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
        );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
