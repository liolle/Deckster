using Blazor.Components;
using Blazor.services;
using Blazor.services.game;
using DotNetEnv;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add Env &  Json configuration
Env.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(configuration);

string SHARED_KEYS = configuration["SHARED_KEYS"] ?? throw new Exception("Missing configuration:\n- SHARED_KEYS");
string HUB_ENDPOINT = configuration["HUB_ENDPOINT"] ?? throw new Exception("Missing configuration:\n- HUB_ENDPOINT");
string API_URL = configuration["API_URL"] ?? throw new Exception("Missing configuration exception:\n- API_URL");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataProtection()
    .SetApplicationName("deckster").PersistKeysToFileSystem(new DirectoryInfo(SHARED_KEYS));

builder.Services.AddHttpClient("main_api", client =>
{
    client.BaseAddress = new Uri(API_URL);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        CookieContainer = new(),
        UseCookies = true
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddSingleton<CSRFService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<HttpInfoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICardsService, CardsService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ClockService>();

builder.Services.AddScoped<MatchService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseCSRFBlazorServer();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GameHubService>($"/{HUB_ENDPOINT}");

app.Run();
