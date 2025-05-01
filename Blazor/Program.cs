using System.Net;
using Blazor.Components;
using Blazor.services;
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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataProtection()
    .SetApplicationName("deckster").PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["SHARED_KEYS"] ?? "/data/keys"));

builder.Services.AddHttpClient("main_api", client =>
{
    string API_URL = configuration["API_URL"] ?? throw new Exception("Missing configuration exception:\n- API_URL");
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

builder.Services.AddSingleton<CSRFService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<HttpInfoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICardsService, CardsService>();
builder.Services.AddScoped<ToastService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseCSRFBlazorServer();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
