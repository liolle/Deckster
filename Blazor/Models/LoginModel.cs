using System.ComponentModel.DataAnnotations;

namespace Blazor.models;

public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = "";


    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

}

public class LoginResult
{

}