using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users.ValueObjects;
using EventManager.Infrastructure.Authentication;
using EventManager.Infrastructure.Bootstrap;
using EventManager.Infrastructure.Options;
using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(
            options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString(
                        "Database"));
            });

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IJWTTokenService, JWTTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        services.Configure<BootrstrapAdminOptions>(
            configuration.GetSection(
                BootrstrapAdminOptions.SectionName));

        services.AddScoped<DatabaseInitializer>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName)
        );

        return services;
    }
}