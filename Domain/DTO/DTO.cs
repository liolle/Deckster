using System.Security.Claims;

namespace deckster.dto;

public enum EProvider
{
  credential,
  microsoft,
  google
}

public enum ERole
{
  admin,
  tester
}

public class BaseTokenClaims(string id, string email,string nickName, EProvider provider)
{
  public string Id { get; set; } = id;
  public EProvider Provider { get; set; } = provider;
  public string NickName { get; } = nickName;
  public string Email { get; set; } = email;
}

public class CredentialInfoModel(string id, string email, string nickName, string password, DateTime created_At)
{
  public string Id { get; } = id;
  public string Email { get; } = email;
  public string NickName { get; } = nickName;
  public string Password { get; } = password;
  public List<ERole> Roles {get; private set;} = [] ;
  public DateTime Created_At { get; } = created_At;

  public void AddRole(List<ERole> roles)
  {
    foreach (ERole r in roles)
    {
      Console.WriteLine(Enum.GetName(r)); 
    }
    Roles = roles;
  }

  public void AddRole(ERole role)
  {
    Roles.Add(role);
  }

  public List<Claim>  GetClaims(){
    List<Claim> claims =
      [
      new(nameof(BaseTokenClaims.Id), Id.ToString()),
      new(nameof(BaseTokenClaims.Email), Email),
      new(nameof(BaseTokenClaims.NickName), NickName),
      new(nameof(BaseTokenClaims.Provider), EProvider.credential.ToString())
      ];

    claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString() ?? string.Empty)));

    return claims;
  }
}
