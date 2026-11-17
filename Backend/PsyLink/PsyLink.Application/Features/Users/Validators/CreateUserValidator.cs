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
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\d{8}$")
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
        RuleFor(command => command.User.Password)
            .NotEmpty()
            .Matches(PasswordPolicy)
            .WithMessage("La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula y número.");
        RuleFor(command => command.User.RoleId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (roleId, cancellationToken) =>
            {
                var role = await userRepository.GetRoleByIdAsync(roleId, cancellationToken);
                return role is not null && role.Name != RoleNames.SuperAdmin;
            })
            .WithMessage("Debe asignarse un rol válido distinto de SUPER_ADMIN.");
    }
}
