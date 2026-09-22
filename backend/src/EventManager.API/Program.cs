using System.Text;
using EventManager.API.Authentication;
using EventManager.API.Exceptions;
using EventManager.Application;
using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.Security;
using EventManager.Domain.Enums;
using EventManager.Infrastructure;
using EventManager.Infrastructure.Bootstrap;
using EventManager.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = 
    builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing."
    );

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = 
        new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = 
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtOptions.Secret
                    )
                ),

            ValidateIssuer = true,

            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,

            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,

            ClockSkew = 
                TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.Authenticated,
        policy =>
        {
            policy.RequireAuthenticatedUser();
        });

    options.AddPolicy(
        AuthorizationPolicies.AdminOnly,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.RequireRole(
                UserRole.Admin.ToString());
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUser, HttpCurrentUser>();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        builder.Configuration
            .GetConnectionString("Database")!);

var app = builder.Build();

using var scope = app.Services.CreateScope();

var initializer = 
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>();

await initializer.InitializeAsync(CancellationToken.None);

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();