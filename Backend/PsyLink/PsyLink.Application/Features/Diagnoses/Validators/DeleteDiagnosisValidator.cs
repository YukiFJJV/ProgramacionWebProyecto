using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class DeleteDiagnosisValidator : AbstractValidator<DeleteDiagnosisCommand>
{
    public DeleteDiagnosisValidator()
    {
        RuleFor(command => command.DiagnosisId).NotEmpty();
    }
}
