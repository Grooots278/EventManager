using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string Login,
    string Password,
    string FirstName,
    string LastName,
    string Email,
    DateOnly BirthDate,
    Guid? CityId
) : ICommand<AuthenticationResponse>;

