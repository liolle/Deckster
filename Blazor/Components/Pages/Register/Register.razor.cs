
using Blazor.services;
using Blazor.models;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Pages.Register;

public partial class Register : ComponentBase
{
    public RegisterModel Model { get; set; } = new();

    [Inject]
    private IAuthService? Service { get; set; }

    [Inject]
    private NavigationManager? Navigation { get; set; }

    private async Task SubmitValidFrom()
    {
        if (Service is null) { return; }
        RegisterResult result = await Service.Register(Model);
        if (!result.IsSuccess)
        {

            return;
        }
        Navigation?.NavigateTo("/", true);
    }

    public void GoToLoginPage()
    {
        Navigation?.NavigateTo("/login", false, true);
    }

}