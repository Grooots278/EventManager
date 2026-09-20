using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Users.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler
    : ICommandHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currendUser;

    public UpdateProfileCommandHandler(IApplicationDbContext db, ICurrentUser currendUser)
    {
        _db = db;
        _currendUser = currendUser;
    }

    public async Task Handle(
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _currendUser.UserId;

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

        if (command.CityId.HasValue)
        {
            var cityExists = 
                await _db.Cities
                    .AnyAsync(
                        x => x.Id == command.CityId.Value,
                        cancellationToken);

            if (!cityExists)
            {
                throw new NotFoundException(
                    "City was not found.");
            }
        }

        user.Profile.UpdatePersonalInformation(
            FirstName.Create(command.FirstName),
            LastName.Create(command.LastName),
            command.BirthDate);

        user.Profile.ChangeCity(
            command.CityId);

        await _db.SaveChangesAsync(
            cancellationToken);
    }
}