namespace deckster.database.entities; 

public class AccountEntity(string id, string provider, int user_Id, string provider_Id)
{
  public string Id {get;init;} = id;
  public string Provider {get;init;} = provider;
  public int User_Id {get;init;} = user_Id;
  public string Provider_Id {get;init;} = provider_Id;

  public static AccountEntity Create(string provider, int user_Id, string provider_Id){
    return  new(GenerateAccoutId(),provider,user_Id,provider_Id);
  }

  private static string GenerateAccoutId()
  {
    string gui = Guid.NewGuid().ToString().Replace("-","");
    return $"AC{gui}";
  }
}
