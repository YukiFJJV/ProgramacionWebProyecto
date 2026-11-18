using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class DeleteRecordDiagnosisValidator : AbstractValidator<DeleteRecordDiagnosisCommand>
{
    public DeleteRecordDiagnosisValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.DiagnosisId).NotEmpty();
    }
}
