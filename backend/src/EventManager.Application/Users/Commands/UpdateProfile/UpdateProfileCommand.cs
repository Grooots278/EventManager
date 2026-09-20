using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    Guid? CityId
) : ICommand;