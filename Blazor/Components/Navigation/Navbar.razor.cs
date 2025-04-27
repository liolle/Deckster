namespace Blazor.Components.Navigation;

using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazor.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class Navbar : ComponentBase
{

    [Inject]
    private NavigationManager? Navigation { get; set; }
    public bool IsConnected { get; set; }

    //[Inject]
    //private AuthenticationStateProvider? AuthProvider { get; set; }
    [Inject]
    private IAuthService? Auth { get; set; }

    public string Page { get; private set; } = "";

    protected override void OnInitialized()
    {
        SetPage();
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.CompletedTask;
        /*
        if (AuthProvider is null) { return; }
        AuthenticationState authState = await AuthProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;
        */
    }

    private void SetPage()
    {
        if (Navigation is null) { return; }

        string[] parts = Navigation.Uri.Split(':');
        if (parts.Length == 0) { return; }
        string pattern = @"\/([^\n\/]*)";
        string input = parts[parts.Length - 1];
        Match match = Regex.Match(input, pattern);
        Page = match.Groups[1].Value.ToLower();
    }

    public void NavigateToHomePage()
    {
        Navigation?.NavigateTo("/");
    }

    public void Login()
    {
        Navigation?.NavigateTo("/login");
    }

    public async Task Logout()
    {
        if (Auth is null) { return; }

        if (await Auth.Logout())
        {
            Navigation?.Refresh(true);
        }
    }
}