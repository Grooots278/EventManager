namespace EventManager.API.Contracts.Authentication;

public sealed record RegisterRequest
(
    string Login, 
    string Password,
    string FirstName,
    string LastName,
    string Email,
    DateOnly BirthDay,
    Guid? CityId
);