using Blazor.Components;
using Microsoft.AspNetCore.DataProtection;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataProtection()
    .SetApplicationName("deckster").PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["SHARED_KEYS"] ?? "/data/keys"));

var app = builder.Build();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
