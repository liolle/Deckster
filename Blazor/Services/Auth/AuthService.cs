using Blazor.models;
using Microsoft.JSInterop;

namespace Blazor.services;

public interface IAuthService
{
    User? GetUser();
    Task<RegisterResult> Register(RegisterModel model);
    Task<bool> CredentialLogin(LoginModel model);
    Task Logout();
}

public class AuthService : IAuthService
{
    private User? CurrentUser;


    private readonly IJSRuntime JS;


    public AuthService(IJSRuntime jS, IConfiguration config)
    {
        JS = jS;
        string API_URL = config["API_URL"] ?? throw new Exception("Missing configuration API_URL");
        _ = JS.InvokeVoidAsync("setURL", API_URL);
    }

    public User? GetUser()
    {
        return CurrentUser;
    }
    private async Task Auth()
    {
        var response = await JS.InvokeAsync<User>("auth");
        CurrentUser = response;

    }
    public async Task<RegisterResult> Register(RegisterModel model)
    {
        RegisterResult result = await JS.InvokeAsync<RegisterResult>("register", model.UserName, model.Password, model.Email, model.NickName);
        Console.WriteLine($"{result.IsSuccess} : {result.ErrorMessage}");
        return result;
    }

    public async Task<bool> CredentialLogin(LoginModel model)
    {
        return await JS.InvokeAsync<bool>("login", model.UserName, model.Password);
    }

    public async Task Logout()
    {
        await JS.InvokeAsync<bool>("logout");
    }
}