using FluentValidation;

namespace EventManager.Application.Authentication.Commands.Logout;

public sealed class LogoutCommandValidator :
    AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
    }
}