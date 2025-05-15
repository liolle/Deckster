using Blazor.Components;
using Blazor.services;
using Blazor.services.game;
using DotNetEnv;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Shared.exceptions;
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add Env &  Json configuration
Env.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(configuration);

string SHARED_KEYS = configuration["SHARED_KEYS"] ?? throw new MissingConfigException("SHARED_KEYS");
string HUB_ENDPOINT = configuration["HUB_ENDPOINT"] ?? throw new MissingConfigException("HUB_ENDPOINT");
string API_URL = configuration["API_URL"] ?? throw new MissingConfigException("API_URL");

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

// Websocket server
builder.Services.AddSignalR();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddSingleton<ConnectionManager>();

// CSRF
builder.Services.AddSingleton(typeof(Program).Assembly);
builder.Services.AddSingleton<CSRFService>();

// Auth 
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
