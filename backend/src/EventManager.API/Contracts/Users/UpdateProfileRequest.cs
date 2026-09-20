namespace EventManager.API.Contracts.Users;

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    Guid? CityId
);