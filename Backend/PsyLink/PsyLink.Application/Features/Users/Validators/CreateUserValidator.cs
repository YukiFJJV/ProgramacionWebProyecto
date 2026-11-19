using FluentValidation;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Validators;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private const string PasswordPolicy = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    public CreateUserValidator(IUserRepository userRepository)
    {
        RuleFor(command => command.User).NotNull();
        RuleFor(command => command.User.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
                !await userRepository.ExistsByEmailAsync(email, cancellationToken))
            .WithMessage("Ya existe una cuenta con ese correo electrónico.");
        RuleFor(command => command.User.FirstName).NotEmpty();
        RuleFor(command => command.User.LastName).NotEmpty();
        RuleFor(command => command.User.PhoneNumber)
            .Matches(@"^\d{8}$")
            .When(command => !string.IsNullOrWhiteSpace(command.User.PhoneNumber))
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
        RuleFor(command => command.User.Password)
            .NotEmpty()
            .Matches(PasswordPolicy)
            .WithMessage("La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula y número.");
    }
}
