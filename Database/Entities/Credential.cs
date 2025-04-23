
namespace deckster.database.entities;


public class CredentialEntity(string account_Id, string user_name, string password)
{
  public string Account_Id {get;init;} = account_Id;
  public string User_name {get;init;} = user_name;
  public string Password {get;init;} = password;
}

