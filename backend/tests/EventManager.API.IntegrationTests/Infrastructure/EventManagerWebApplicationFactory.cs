using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventManager.API.IntegrationTests.Infrastructure;

public sealed class EventManagerWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    
    public EventManagerWebApplicationFactory(string connectionString)
        => _connectionString = connectionString;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(
            configuration =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] = _connectionString,
                        ["Jwt:Issuer"] = "EventManager.Tests",
                        ["Jwt:Audience"] = "EventManager.Api.Tests",
                        ["Jwt:Secret"] = "test-secret-key-that-is-long-enough-123456789",
                        ["Jwt:AccessTokenMinutes"] = "15",
                        ["Jwt:RefreshTokenDays"] = "30",
                        ["BootstrapAdmin:Enabled"] = "false"
                    });
            }
        );

        builder.UseEnvironment("Testing");

        return base.CreateHost(builder);
    }
}