using FluentValidation;
using PsyLink.Application.Features.Psychologists.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Psychologists.Validators;

public sealed class RegisterPsychologistValidator : AbstractValidator<RegisterPsychologistCommand>
{
    private const string PasswordPolicy = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    public RegisterPsychologistValidator(
        IUserRepository userRepository,
        IPsychologistRepository psychologistRepository,
        ISpecialtyRepository specialtyRepository)
    {
        RuleFor(command => command.Psychologist).NotNull();
        RuleFor(command => command.Psychologist.User.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
                !await userRepository.ExistsByEmailAsync(email, cancellationToken))
            .WithMessage("Ya existe una cuenta con ese correo electrónico.");
        RuleFor(command => command.Psychologist.User.FirstName).NotEmpty();
        RuleFor(command => command.Psychologist.User.LastName).NotEmpty();
        RuleFor(command => command.Psychologist.User.Password)
            .NotEmpty()
            .Matches(PasswordPolicy)
            .WithMessage("La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula y número.");
        RuleFor(command => command.Psychologist.User.PhoneNumber)
            .Matches(@"^\d{8}$")
            .When(command => !string.IsNullOrWhiteSpace(command.Psychologist.User.PhoneNumber))
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
        RuleFor(command => command.Psychologist.SpecialtyId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (specialtyId, cancellationToken) =>
                await specialtyRepository.GetByIdAsync(specialtyId, cancellationToken) is not null)
            .WithMessage("La especialidad seleccionada no existe.");
        RuleFor(command => command.Psychologist.LicenseNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6)
            .MustAsync(async (licenseNumber, cancellationToken) =>
                !await psychologistRepository.ExistsByLicenseNumberAsync(licenseNumber, cancellationToken))
            .WithMessage("El número de licencia ya está registrado.");
    }
}
