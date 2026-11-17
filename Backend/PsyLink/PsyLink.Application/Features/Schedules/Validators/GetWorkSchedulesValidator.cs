using FluentValidation;
using PsyLink.Application.Features.Schedules.Services.Queries;

namespace PsyLink.Application.Features.Schedules.Validators;

public sealed class GetWorkSchedulesValidator : AbstractValidator<GetWorkSchedulesQuery>
{
    public GetWorkSchedulesValidator()
    {
        RuleFor(query => query.PsychologistId).NotEmpty();
    }
}
