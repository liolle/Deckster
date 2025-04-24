using deckster.entities;

namespace deckster.extensions;

public static partial class EntityExtension
{
  public static AccountEntity Create(this AccountEntity account,string provider, int user_Id, string provider_Id){
    return  new(GenerateAccoutId("AC"),provider,user_Id,provider_Id);
  }

  public static UserEntity Create(this UserEntity account,string email){
    return  new(GenerateAccoutId("USR"),email,DateTime.Now);
  }

  public static CredentialEntity Create(this CredentialEntity account,string user_name, string password){
    return  new(GenerateAccoutId("CRD"),user_name,password);
  }
}

// Helper methods
public static partial class EntityExtension
{
  private static string GenerateAccoutId(string prefix)
  {
    string gui = Guid.NewGuid().ToString().Replace("-","");
    return $"{prefix}{gui}";
  }
}
