using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Users.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : ICommand;
    