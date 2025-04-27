namespace Blazor.models;

public class User(string id, string email, string nickname)
{
    public string Id { get; set; } = id;
    public string Email { get; set; } = email;
    public string Nickname { get; set; } = nickname;
}