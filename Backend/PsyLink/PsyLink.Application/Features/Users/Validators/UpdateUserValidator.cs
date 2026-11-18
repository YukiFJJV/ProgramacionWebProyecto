using FluentValidation;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Validators;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator(IUserRepository userRepository)
    {
        RuleFor(command => command)
            .Must(command =>
                command.EmailSpecified ||
                command.FirstNameSpecified ||
                command.LastNameSpecified ||
                command.PhoneNumberSpecified ||
                command.RoleIdSpecified ||
                command.ActiveSpecified)
            .WithMessage("Debe especificar al menos un campo para actualizar.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .When(command => command.EmailSpecified)
            .WithMessage("El correo electrónico no tiene un formato válido.");
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
                !command.EmailSpecified ||
                string.IsNullOrWhiteSpace(command.Email) ||
                !await userRepository.ExistsByEmailAsync(
                    command.Email,
                    command.UserId,
                    cancellationToken))
            .WithMessage("Ya existe una cuenta con ese correo electrónico.");

        RuleFor(command => command.FirstName)
            .NotEmpty()
            .When(command => command.FirstNameSpecified);
        RuleFor(command => command.LastName)
            .NotEmpty()
            .When(command => command.LastNameSpecified);
        RuleFor(command => command.PhoneNumber)
            .Must(phone => phone is null || System.Text.RegularExpressions.Regex.IsMatch(
                phone,
                @"^\d{8}$"))
            .When(command => command.PhoneNumberSpecified)
            .WithMessage("El teléfono debe contener exactamente 8 dígitos o ser null para limpiarlo.");
        RuleFor(command => command.RoleId)
            .NotNull()
            .When(command => command.RoleIdSpecified);
        RuleFor(command => command.RoleId)
            .MustAsync(async (roleId, cancellationToken) =>
            {
                if (!roleId.HasValue)
                {
                    return true;
                }

                var role = await userRepository.GetRoleByIdAsync(roleId.Value, cancellationToken);
                return role is not null && role.Name != RoleNames.SuperAdmin;
            })
            .When(command => command.RoleIdSpecified)
            .WithMessage("Debe asignarse un rol válido distinto de SUPER_ADMIN.");
        RuleFor(command => command.Active)
            .NotNull()
            .When(command => command.ActiveSpecified);
    }
}
