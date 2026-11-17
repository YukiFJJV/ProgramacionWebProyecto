using FluentValidation;
using PsyLink.Application.Features.PatientRecords.Services.Commands;

namespace PsyLink.Application.Features.PatientRecords.Validators;

public sealed class UpdatePatientRecordValidator : AbstractValidator<UpdatePatientRecordCommand>
{
    public UpdatePatientRecordValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.Update).NotNull();
        When(request => request.Update is not null, () =>
        {
            RuleFor(request => request.Update.RiskScore)
                .NotNull()
                .WithMessage("Debe especificar el nuevo nivel de riesgo.");
            RuleFor(request => request.Update.RiskScore)
                .InclusiveBetween(0, 3)
                .When(request => request.Update.RiskScore.HasValue);
        });
    }
}
