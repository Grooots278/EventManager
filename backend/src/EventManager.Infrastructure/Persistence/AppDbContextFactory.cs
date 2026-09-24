using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EventManager.Infrastructure.Persistence;

public sealed class AppDbContextFactory :
    IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(
        string[] args)
    {
        var configuration = 
            new ConfigurationBuilder()
                .SetBasePath(
                    Directory.GetCurrentDirectory())
                .AddJsonFile(
                    "appsettings.json",
                    optional: true)
                .AddJsonFile(
                    "appsettings.Development.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build();

        var connectionString = 
            configuration.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = 
                Environment.GetEnvironmentVariable(
                    "ConnectionStrings__Database");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Database connection string is not configured.");

        var optionsBuilder = 
            new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(
            optionsBuilder.Options);
    }
}