using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Authentication.Commands.Refresh;

public sealed record RefreshCommand(
    string RefreshToken
) : ICommand<AuthenticationResponse>;