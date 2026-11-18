using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Queries;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class GetDiagnosisDetailValidator : AbstractValidator<GetDiagnosisDetailQuery>
{
    public GetDiagnosisDetailValidator()
    {
        RuleFor(request => request.DiagnosisId).NotEmpty();
    }
}
