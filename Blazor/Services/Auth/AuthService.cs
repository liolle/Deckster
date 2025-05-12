using Blazor.models;
using Microsoft.JSInterop;

namespace Blazor.services;

public interface IAuthService
{
    Task<RegisterResult> Register(RegisterModel model);
    Task<bool> CredentialLogin(LoginModel model);
    Task<bool> Logout();
}

public class AuthService : IAuthService
{

    private readonly IJSRuntime JS;

    public AuthService(IJSRuntime jS, IConfiguration config, ToastService toast)
    {
        JS = jS;
        string INTER_URL = config["INTER_URL"] ?? throw new Exception("Missing configuration INTER_URL"); ;
        _ = JS.InvokeVoidAsync("setURL", INTER_URL);
    }



    public async Task<RegisterResult> Register(RegisterModel model)
    {
        RegisterResult result = await JS.InvokeAsync<RegisterResult>("register", model.UserName, model.Password, model.Email, model.NickName);
        return result;
    }

    public async Task<bool> CredentialLogin(LoginModel model)
    {
        return await JS.InvokeAsync<bool>("login", model.UserName, model.Password);
    }

    public async Task<bool> Logout()
    {
        return await JS.InvokeAsync<bool>("logout");
    }
}
