using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class UpsertRecordDiagnosisValidator : AbstractValidator<UpsertRecordDiagnosisCommand>
{
    public UpsertRecordDiagnosisValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.DiagnosisId).NotEmpty();
        RuleFor(request => request.Diagnosis).NotNull();
        When(request => request.Diagnosis is not null, () =>
        {
            RuleFor(request => request.Diagnosis.Notes).MaximumLength(5_000);
        });
    }
}
