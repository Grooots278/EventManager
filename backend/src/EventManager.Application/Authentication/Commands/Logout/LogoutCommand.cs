using EventManager.Application.Common.CQRS;
using MediatR;

namespace EventManager.Application.Authentication.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken
) : ICommand<Unit>;