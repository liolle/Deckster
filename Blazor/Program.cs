using Blazor.Components;
using Blazor.services;
using DotNetEnv;
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ToastService>();

var app = builder.Build();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
