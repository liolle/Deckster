// Packages
using Microsoft.AspNetCore.Identity;
using DotNetEnv;

// Refs
using deckster.database;
using deckster.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using deckster.exceptions;
using edllx.dotnet.csrf;
using Microsoft.AspNetCore.DataProtection;

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
        string jwt_key = configuration["JWT_KEY"] ?? throw new MissingConfigException("JWT_KEY");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT_ISSUER"],
            ValidAudience = configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key))
        };

        // extract token from cookies and place it into the Authorization header.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string jwt_name = configuration["AUTH_TOKEN_NAME"] ?? throw new MissingConfigException("AUTH_TOKEN_NAME");
                if (context.Request.Cookies.ContainsKey(jwt_name))
                {
                    context.Token = context.Request.Cookies[jwt_name];
                }
                return Task.CompletedTask;
            }
        };
    });

string front_host = configuration["FRONT_HOST"] ?? throw new MissingConfigException("FRONT_HOST");

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCredentials", policy =>
    {
        policy
        .WithOrigins([front_host, "https://localhost:7257"])
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
