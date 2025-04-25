using deckster.utils;

namespace deckster.entities;

public class UserEntity (string id, string email,DateTime createdAt,string nickName)
{
  public string Id {get;init;} = id;
  public string Email {get;init;} = email;
  public string NickName {get;init;} = nickName;

  public DateTime Created_At {get;init;} = createdAt;
    public static UserEntity Create(string email,string nickName){
    return  new(IdGererator.GenerateId("USR"),email,DateTime.Now,nickName);
  }
}

