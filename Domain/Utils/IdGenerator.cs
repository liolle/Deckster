namespace deckster.utils;

public static class IdGererator
{
  public static string GenerateId(string prefix)
  {
    string gui = Guid.NewGuid().ToString().Replace("-","");
    return $"{prefix}{gui}";
  }
}
