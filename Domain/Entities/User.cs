namespace deckster.entities;

public class UserEntity (string id, string email,DateTime createdAt)
{
  public string Id {get;init;} = id;
  public string Email {get;init;} = email;
  public DateTime Created_At {get;init;} = createdAt;
}

