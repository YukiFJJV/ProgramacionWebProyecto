using FluentValidation;
using PsyLink.Application.Features.Patients.Services.Commands;

namespace PsyLink.Application.Features.Patients.Validators;

public sealed class UpdatePatientProfileValidator : AbstractValidator<UpdatePatientProfileCommand>
{
    public UpdatePatientProfileValidator()
    {
        RuleFor(command => command.PatientId).NotEmpty();
        RuleFor(command => command)
            .Must(command =>
                command.FirstNameSpecified ||
                command.LastNameSpecified ||
                command.EmailSpecified ||
                command.PhoneNumberSpecified ||
                command.BirthDateSpecified ||
                command.DescriptionSpecified)
            .WithMessage("Debe especificar al menos un campo para actualizar.");
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .When(command => command.FirstNameSpecified);
        RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .When(command => command.LastNameSpecified);
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .When(command => command.EmailSpecified)
            .WithMessage("El correo electrónico no tiene un formato válido.");
        RuleFor(command => command.PhoneNumber)
            .Must(phone => phone is null || System.Text.RegularExpressions.Regex.IsMatch(
                phone,
                @"^\d{8}$"))
            .When(command => command.PhoneNumberSpecified)
            .WithMessage("El teléfono debe contener exactamente 8 dígitos o ser null para limpiarlo.");
        RuleFor(command => command.BirthDate)
            .NotNull()
            .When(command => command.BirthDateSpecified);
        RuleFor(command => command)
            .Must(command =>
                !command.BirthDateSpecified ||
                command.BirthDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La fecha de nacimiento no puede ser futura.");
    }
}
