using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Redirect.RedirectOnClaim;


public partial class RedirectOnClaim : ComponentBase
{
    [Parameter, EditorRequired]
    public string? ClaimName { get; set; }
    [Parameter]
    public string? ClaimValue { get; set; }

    [Parameter, EditorRequired]
    public string RedirectOnFailure { get; set; } = "/";

    [Parameter]
    public string RedirectOnValid { get; set; } = "";

    [Inject]
    AuthenticationStateProvider? AuthProvider { get; set; }

    private bool IsValid { get; set; } = false;

    public bool ValidateClaims(AuthenticationState context)
    {
        if (ClaimName is null) { return true; }
        if (ClaimValue is null)
        {
            return context.User.HasClaim(value => value.Type == ClaimName);
        }
        return context.User.HasClaim(value => value.Type == ClaimName && value.Value == ClaimValue);
    }

    protected override async Task OnInitializedAsync()
    {
        if (AuthProvider is null) { return; }
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        IsValid = ValidateClaims(authState);

        if (!IsValid)
        {
            handleRedirect(RedirectOnFailure);
            return;
        }

        if (!string.IsNullOrEmpty(RedirectOnValid))
        {
            navigation?.NavigateTo(RedirectOnValid);
        }
    }

    private void handleRedirect(string url)
    {
        Match matcher = Regex.Match(navigation?.Uri ?? "", $@"(https|http):\/\/[a-zA-Z0-9.:]*({url}[^\/]*$)");


        if (matcher.Groups.Values.Last().Value == url)
        {
            return;
        }


        navigation?.NavigateTo(url);
    }
}