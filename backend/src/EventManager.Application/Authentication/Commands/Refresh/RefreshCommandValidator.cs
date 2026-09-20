using FluentValidation;

namespace EventManager.Application.Authentication.Commands.Refresh;

public sealed class RefreshCommandValidator
    : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
    }
}