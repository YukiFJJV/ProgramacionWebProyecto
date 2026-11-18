using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class UpsertPatientDiagnosisValidator : AbstractValidator<UpsertPatientDiagnosisCommand>
{
    public UpsertPatientDiagnosisValidator()
    {
        RuleFor(command => command.PatientId).NotEmpty();
        RuleFor(command => command.DiagnosisId).NotEmpty();
        RuleFor(command => command.StatusId).NotEmpty();
        RuleFor(command => command.Notes)
            .Must(string.IsNullOrWhiteSpace)
            .WithMessage("Las notas no se pueden guardar porque patient_diagnoses no tiene una columna para ellas.");
    }
}
