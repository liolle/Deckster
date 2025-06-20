// Packages
using Microsoft.AspNetCore.Identity;
using DotNetEnv;

// Refs
using deckster.database;
using deckster.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.DataProtection;
using Shared.exceptions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add Env &  Json configuration
Env.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(configuration);


builder.Services.AddDataProtection()
    .SetApplicationName("deckster").PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["SHARED_KEYS"] ?? "/data/keys"));

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string jwtKey = configuration["JWT_KEY"] ?? throw new MissingConfigException("JWT_KEY");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT_ISSUER"],
            ValidAudience = configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // extract token from cookies and place it into the Authorization header.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string jwtName = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
                if (context.Request.Cookies.ContainsKey(jwtName))
                {
                    context.Token = context.Request.Cookies[jwtName];
                }
                return Task.CompletedTask;
            }
        };
    });

string frontHost = configuration["FRONT_HOST"] ?? throw new MissingConfigException("FRONT_HOST");

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCredentials", policy =>
    {
        policy
        .WithOrigins([frontHost, "https://localhost:7257"])
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});



builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
builder.Services.AddControllers();
builder.Services.AddSingleton<CSRFService>();

builder.Services.AddScoped<IDataContext, DataContext>();
builder.Services.AddScoped<IJWTService, JwtService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICardService, CardService>();

var app = builder.Build();

app.UseCors();
app.UseAuthorization();
app.UseCSRFApi();

app.MapControllers();

app.Run();
