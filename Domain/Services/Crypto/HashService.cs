using Microsoft.AspNetCore.Identity;

namespace deckster.services;

public interface IHashService
{
  public string HashPassword(string password);
  public bool VerifyPassword(string hashedPassword, string password);
}

public class HashService(IPasswordHasher<string> passwordHasher) : IHashService
{
  private readonly IPasswordHasher<string> _passwordHasher = passwordHasher;

  public string HashPassword(string password)
  {
    return _passwordHasher.HashPassword(string.Empty, password);
  }

  public bool VerifyPassword(string hashedPassword, string password)
  {
    var result = _passwordHasher.VerifyHashedPassword(string.Empty, hashedPassword, password);
    return result == PasswordVerificationResult.Success;
  }
}
