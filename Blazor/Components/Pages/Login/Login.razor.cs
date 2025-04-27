using Blazor.services;
using Blazor.models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Login;

public partial class Login : ComponentBase
{
    public LoginModel Model { get; set; } = new();

    [Inject]
    private IAuthService? Service { get; set; }

    [Inject]
    private NavigationManager? Navigation { get; set; }

    private async Task SubmitValidFrom()
    {
        if (Service is null) { return; }
        var result = await Service.CredentialLogin(Model);
        if (!result) { return; }
        Navigation?.NavigateTo("/", true);
    }

    public void GoToRegisterPage()
    {
        Navigation?.NavigateTo("/register", false, true);
    }

}