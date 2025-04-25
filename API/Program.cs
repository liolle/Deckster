// Packages
using Microsoft.AspNetCore.Identity;
using DotNetEnv;

// Refs
using deckster.database;
using deckster.services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add Env &  Json configuration
Env.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
builder.Services.AddControllers();

builder.Services.AddScoped<IDataContext,DataContext>();
builder.Services.AddScoped<IJWTService,JwtService>();
builder.Services.AddScoped<IHashService,HashService>();
builder.Services.AddScoped<IAuthService,AuthService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
