using System.Security.Claims;
using System.Text.Json.Serialization;
using deckster.entities;

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

public class BaseTokenClaims(string id, string email, string nickName, EProvider provider)
{
  public string Id { get; set; } = id;
  public EProvider Provider { get; set; } = provider;
  public string NickName { get; } = nickName;
  public string Email { get; set; } = email;
}

public class CredentialInfoModel(string id, string account_id, string email, string nickName, string password, DateTime created_At)
{
  [JsonPropertyName("id")]
  public string Id { get; } = id;

  [JsonPropertyName("accountId")]
  public string AccountId { get; } = account_id;
  [JsonPropertyName("email")]
  public string Email { get; } = email;
  [JsonPropertyName("nickname")]
  public string NickName { get; } = nickName;
  [JsonPropertyName("password")]
  public string Password { get; } = password;
  [JsonPropertyName("role")]
  public List<ERole> Roles { get; private set; } = [];
  [JsonPropertyName("createdAt")]
  public DateTime Created_At { get; } = created_At;

  public void AddRole(List<ERole> roles)
  {
    Roles = roles;
  }

  public void AddRole(ERole role)
  {
    Roles.Add(role);
  }

  public List<Claim> GetClaims()
  {
    List<Claim> claims =
      [
      new(nameof(BaseTokenClaims.Id), Id.ToString()),
      new("AccountId", AccountId.ToString()),
      new(nameof(BaseTokenClaims.Email), Email),
      new(nameof(BaseTokenClaims.NickName), NickName),
      new(nameof(BaseTokenClaims.Provider), EProvider.credential.ToString())
      ];

    claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString() ?? string.Empty)));

    return claims;
  }
}


public class DeckModel
{
  [JsonPropertyName("deck")]
  public DeckEntity Deck { get; init; }

  [JsonPropertyName("cards")]
  public List<DeckCardEntity> Cards { get; set; }


  public DeckModel(DeckEntity deck)
  {
    Deck = deck;
    Cards = [];
  }

  public DeckModel(DeckEntity deck, List<DeckCardEntity> cards)
  {
    Deck = deck;
    Cards = cards;
  }
}

public class DeckDTO
{
  [JsonPropertyName("deckId")]
  string DeckId { get; init; } = "";

  [JsonPropertyName("cards")]
  List<CardEntity> Cards { get; set; } = [];

}