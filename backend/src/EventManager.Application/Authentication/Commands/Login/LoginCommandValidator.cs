using FluentValidation;

namespace EventManager.Application.Authentication.Commands.Login;

public sealed class LoginCommandValidator :
    AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}