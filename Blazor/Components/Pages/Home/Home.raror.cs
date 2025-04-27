using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Pages.Home;

public partial class Home : ComponentBase, IDisposable
{
    string CurrentUserId = "";

    [Inject]
    private AuthenticationStateProvider? AuthProvider { get; set; }

    [Inject]
    private NavigationManager? navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthProvider is null) { return; }
        AuthenticationState authState = await AuthProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (!(user.Identity?.IsAuthenticated ?? false)) { return; }

        CurrentUserId = user.FindFirst("Id")?.Value ?? "";
        if (String.IsNullOrEmpty(CurrentUserId)) { return; }
    }

    private void HandleJoinGame()
    {
        Console.WriteLine("Join Game");
    }

    public void SearchGame()
    {
        Console.WriteLine("Join Game");
    }

    public void Dispose()
    {
    }
}