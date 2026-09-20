using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Authentication;
using EventManager.Domain.Cities;
using EventManager.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence;

public sealed class AppDbContext : 
    DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.
            ApplyConfigurationsFromAssembly(typeof(AppDbContext)
            .Assembly);
    }
}
