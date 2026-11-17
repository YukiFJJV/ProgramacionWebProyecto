using FluentValidation;
using PsyLink.Application.Features.Patients.Services.Commands;

namespace PsyLink.Application.Features.Patients.Validators;

public sealed class RegisterPatientValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientValidator()
    {
        RuleFor(command => command.Patient).NotNull();
        RuleFor(command => command.Patient.UserId).NotEmpty();
        RuleFor(command => command.Patient.BirthDate)
            .Must(birthDate => birthDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La fecha de nacimiento no puede ser futura.");
    }
}
