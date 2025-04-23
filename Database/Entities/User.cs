namespace deckster.database.entities;

public class UserEntity (string id, string email,DateTime createdAt)
{
  public string Id {get;init;} = id;
  public string Email {get;init;} = email;
  public DateTime Created_At {get;init;} = createdAt;

  public static UserEntity Create(string email, DateTime createdAt){
    return new UserEntity( CreateUserId(),  email, createdAt);
  }

  private static string CreateUserId()
  {
    string gui = Guid.NewGuid().ToString().Replace("-","");
    return $"USR{gui}";
  }
}

