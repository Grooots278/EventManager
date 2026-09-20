using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using EventManager.Application.Users.DTO;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IQueryHandler<
        GetCurrentUserQuery,
        CurrentUserResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CurrentUserResponse> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = 
            await _db.Users
                .Include(x => x.Profile)
                .SingleOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "User was not found.");
        }

        string? cityName = null;

        if (user.Profile.CityId.HasValue)
        {
            cityName = 
                await _db.Cities
                    .Where(x => 
                        x.Id == user.Profile.CityId.Value)
                    .Select(x => x.Name)
                    .SingleOrDefaultAsync(
                        cancellationToken);
        }

        return new CurrentUserResponse(
            user.Id,
            user.Login.Value,
            user.Profile.FirstName.Value,
            user.Profile.LastName.Value,
            user.Profile.Email.Value,
            user.Profile.BirthDate,
            user.Profile.CityId,
            cityName,
            user.Profile.Role,
            user.Profile.IsActive
        );
    }
}