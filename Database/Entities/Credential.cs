
namespace deckster.database.entities;


public class CredentialEntity(string account_Id, string user_name, string password)
{
  public string Account_Id {get;init;} = account_Id;
  public string User_name {get;init;} = user_name;
  public string Password {get;init;} = password;

  public static CredentialEntity Create(string user_name, string password){
    return new CredentialEntity(GenerateCredId(),user_name,password);
  }

  private static string GenerateCredId()
  {
    string gui = Guid.NewGuid().ToString().Replace("-","");
    return $"CRD{gui}";
  }
}

