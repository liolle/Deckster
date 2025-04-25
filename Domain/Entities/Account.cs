using deckster.utils;

namespace deckster.entities; 

public class AccountEntity(string id, string provider, string user_Id, string provider_Id)
{
  public string Id {get;init;} = id;
  public string Provider {get;init;} = provider;
  public string User_Id {get;init;} = user_Id;
  public string Provider_Id {get;init;} = provider_Id;

  public static AccountEntity Create(string provider, string user_Id, string provider_Id){
    return  new(IdGererator.GenerateId("AC"),provider,user_Id,provider_Id);
  }
}
