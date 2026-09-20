using EventManager.Domain.Enums;

namespace EventManager.Application.Users.DTO;

public sealed record CurrentUserResponse(
    Guid Id,
    string Login,
    string FirstName,
    string LastName,
    string Email,
    DateOnly BirthDate,
    Guid? CityId,
    string? CityName,
    UserRole Role,
    bool IsActive
);