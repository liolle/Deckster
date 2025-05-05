using Blazor.models;
using Blazor.services.game;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Components.Redirect.Loading;

public partial class Loading : ComponentBase, IDisposable
{
    [Inject]
    private MatchService? _matchService { get; set; }

    [Inject]
    private NavigationManager? navigation { get; set; }

    private bool IsLoading { get; set; } = true;
    private bool Cancelled { get; set; } = false;

    public bool ValidateClaims(AuthenticationState context)
    {
        return context.User.HasClaim(value => value.Type == "Id");
    }

    protected override void OnInitialized()
    {
        if (_matchService is null)
        {
            return;
        }
        IsLoading = _matchService.IsLoading;

        _matchService.JoinGame += HandleJoinGame;

        Init();
    }

    private async void Init()
    {
        await Task.Delay(3000);
        if (!Cancelled)
        {
            StopLoading();
        }
    }

    private void StopLoading()
    {
        IsLoading = false;
        if (_matchService is not null)
        {
            _matchService.IsLoading = false;
        }
        StateHasChanged();
    }

    private void HandleJoinGame(GameMatch gameMatch, Player player)
    {
        Cancelled = true;
        StopLoading();
        navigation?.NavigateTo("/game");
    }

    public void Dispose()
    {
        if (_matchService is not null)
        {
            _matchService.JoinGame -= HandleJoinGame;
        }
    }
}