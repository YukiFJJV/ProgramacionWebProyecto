using FluentValidation;
using PsyLink.Application.Features.PatientRecords.Services.Queries;

namespace PsyLink.Application.Features.PatientRecords.Validators;

public sealed class GetPatientRecordValidator : AbstractValidator<GetPatientRecordQuery>
{
    public GetPatientRecordValidator()
    {
        RuleFor(request => request.PatientId).NotEmpty();
    }
}
