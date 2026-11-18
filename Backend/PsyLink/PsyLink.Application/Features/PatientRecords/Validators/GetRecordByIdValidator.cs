using FluentValidation;
using PsyLink.Application.Features.PatientRecords.Services.Queries;

namespace PsyLink.Application.Features.PatientRecords.Validators;

public sealed class GetRecordByIdValidator : AbstractValidator<GetRecordByIdQuery>
{
    public GetRecordByIdValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
    }
}
