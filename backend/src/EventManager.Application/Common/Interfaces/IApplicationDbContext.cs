using EventManager.Domain.Authentication;
using EventManager.Domain.Cities;
using EventManager.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<UserProfile> UserProfiles { get; }

    DbSet<City> Cities { get; }

    DbSet<RefreshSession> RefreshSessions { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
