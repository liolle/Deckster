using System.ComponentModel.DataAnnotations;

namespace Blazor.models;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Username can only contain letters, numbers, hyphens and underscores")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Nickname is required")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Nickname must be between 2 and 30 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_\- ]+$", ErrorMessage = "Nickname can only contain letters, numbers, spaces, hyphens and underscores")]
    public string NickName { get; set; } = "";
}

public class RegisterResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = "";
}