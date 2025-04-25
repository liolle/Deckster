using System.ComponentModel.DataAnnotations;
using deckster.cqs;
using deckster.dto;

namespace deckster.services.queries;



public class CredentialLoginQuery(string userName, string password) : IQueryDefinition<string>
{
  [Required(ErrorMessage = "Username is required")]
  [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
  [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Username can only contain letters, numbers, hyphens and underscores")]
  public string UserName { get; set; } = userName;

  [Required(ErrorMessage = "Password is required")]
  [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
  [DataType(DataType.Password)]
  public string Password { get; set; } = password;

}

public class UserFromUserNameQuery(string userName) : IQueryDefinition<CredentialInfoModel?>
{
  public string UserName {get;} = userName;
}
