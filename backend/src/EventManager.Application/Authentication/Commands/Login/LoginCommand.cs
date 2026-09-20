using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Login,
    string Password
) : ICommand<AuthenticationResponse>;