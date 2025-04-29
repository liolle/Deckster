namespace deckster.entities;

public class RoleEntity (string account_id, string role)
{
  public string AccountId {get;init;} = account_id;
  public string Role {get;init;} = role;
}

