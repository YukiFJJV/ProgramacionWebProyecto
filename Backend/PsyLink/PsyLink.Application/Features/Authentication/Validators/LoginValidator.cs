using FluentValidation;
using PsyLink.Application.Features.Authentication.Services.Commands;

namespace PsyLink.Application.Features.Authentication.Validators;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(command => command.Password).NotEmpty();
    }
}
