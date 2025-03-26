using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Api.Models;
using PadelBookingApp.Infrastructure;
using PadelBookingApp.Domain.Entities;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Enable PII logging for debugging purposes (remove in production)
IdentityModelEventSource.ShowPII = true;

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Configure CORS for your React app.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionStringTemplate))
{
    throw new InvalidOperationException("DefaultConnection string is not configured.");
}

var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

var connectionString = connectionStringTemplate
    .Replace("${DB_NAME}", dbName)
    .Replace("${DB_USER}", dbUser)
    .Replace("${DB_PASSWORD}", dbPassword);

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var currentJwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(currentJwtKey))
{
    var envJwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    if (string.IsNullOrEmpty(envJwtKey))
    {
        byte[] keyBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }
        envJwtKey = Convert.ToBase64String(keyBytes);
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", envJwtKey);
    }
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "Jwt:Key", envJwtKey }
    });
}

var keyCheck = builder.Configuration["Jwt:Key"];
Console.WriteLine($"JWT Key: {keyCheck}");

// Bind JWT settings from configuration.
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings not found.");
}
// Use fallback for Audience and Issuer if they're missing.
if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    jwtSettings.Audience = "PadelBookingAppUsers";
    Console.WriteLine("JWT Audience not set. Falling back to 'PadelBookingAppUsers'.");
}
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
{
    jwtSettings.Issuer = "PadelBookingApp";
    Console.WriteLine("JWT Issuer not set. Falling back to 'PadelBookingApp'.");
}

Console.WriteLine($"JWT Audience (Validation): {jwtSettings.Audience}");
Console.WriteLine($"JWT Issuer (Validation): {jwtSettings.Issuer}");

// Create a symmetric security key with an explicit KeyId.
var keyBytesForSigning = Encoding.ASCII.GetBytes(jwtSettings.Key);
var symmetricKey = new SymmetricSecurityKey(keyBytesForSigning)
{
    KeyId = "default_key"
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = symmetricKey,
        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
        {
            return new List<SecurityKey> { symmetricKey };
        }
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token validation failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        var adminUser = new User
        {
            Email = "admin@admin.com",
            Password = BCrypt.Net.BCrypt.HashPassword("admin1234"),
            Role = "Admin"
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();