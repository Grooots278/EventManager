using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;
using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace EventManager.API.IntegrationTests.Infrastructure;

public sealed class ApiFixture 
    : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase(
                "eventmanager_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public EventManagerWebApplicationFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        Factory = 
            new EventManagerWebApplicationFactory(
                _postgres.GetConnectionString());

        Client = Factory.CreateClient();

        await ApplyMigrationsAsync();

        await SeedAdminAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        
        var db = 
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
    }

    private async Task SeedAdminAsync()
    {
        await using var scope = 
            Factory.Services.CreateAsyncScope();

        var db = 
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var passwordHasher = 
            scope.ServiceProvider
                .GetRequiredService<IPasswordHasher>();

        var exists = 
            await db.UserProfiles.AnyAsync
                (x => 
                    x.Email == 
                    Email.Create(
                        "admin@test.local"));

        if (exists)
        {
            return;
        }

        var admin = 
            User.Create(
                Login.Create("admin"),
                PasswordHash.FromHash(
                    passwordHasher.Hash(
                        "AdminPassword123456!")),
                FirstName.Create("System"),
                LastName.Create("Administrator"),
                Email.Create("admin.test.local"),
                new DateOnly(1990, 1, 1),
                null
            );

        await db.Users.AddAsync(admin);

        await db.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();

        await Factory.DisposeAsync();

        await _postgres.DisposeAsync();
    }
}