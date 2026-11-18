using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Queries;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class GetPatientDiagnosesValidator : AbstractValidator<GetPatientDiagnosesQuery>
{
    public GetPatientDiagnosesValidator() =>
        RuleFor(query => query.PatientId).NotEmpty();
}
