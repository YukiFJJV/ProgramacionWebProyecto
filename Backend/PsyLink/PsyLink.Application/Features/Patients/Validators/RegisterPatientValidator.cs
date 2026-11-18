using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class RegisterPatientValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientValidator()
    {
        RuleFor(command => command.Patient).NotNull();
        RuleFor(command => command.Patient.User).NotNull();
        RuleFor(command => command.Patient.User.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(command => command.Patient.User.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula y número.");
        RuleFor(command => command.Patient.User.FirstName).NotEmpty();
        RuleFor(command => command.Patient.User.LastName).NotEmpty();
        RuleFor(command => command.Patient.User.PhoneNumber)
            .Matches(@"^\d{8}$")
            .When(command => !string.IsNullOrWhiteSpace(command.Patient.User.PhoneNumber))
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
        RuleFor(command => command.Patient.BirthDate)
            .NotEqual(DateOnly.MinValue)
            .WithMessage("La fecha de nacimiento es obligatoria.")
            .Must(birthDate => birthDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La fecha de nacimiento no puede ser futura.");
    }
}
