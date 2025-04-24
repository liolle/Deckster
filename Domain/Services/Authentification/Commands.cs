using deckster.cqs;

namespace deckster.services.commands;

public class RegisterUserCommand(string userName, string email, string password, string nickName) : ICommandDefinition
{
  public string UserName { get; set; } = userName;
  public string Email { get; set; } = email;
  public string Password { get; set; } = password;
  public string NickName { get; set; } = nickName;
}
