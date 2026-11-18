using FluentValidation;
using PsyLink.Application.Features.Patients.Services.Queries;

namespace PsyLink.Application.Features.Patients.Validators;

public sealed class GetPatientDiagnosesValidator : AbstractValidator<GetPatientDiagnosesQuery>
{
    public GetPatientDiagnosesValidator() =>
        RuleFor(query => query.PatientId).NotEmpty();
}
