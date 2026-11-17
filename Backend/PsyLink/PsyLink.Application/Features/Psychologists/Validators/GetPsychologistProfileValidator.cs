using FluentValidation;
using PsyLink.Application.Features.Psychologists.Services.Queries;

namespace PsyLink.Application.Features.Psychologists.Validators;

public sealed class GetPsychologistProfileValidator : AbstractValidator<GetPsychologistProfileQuery>
{
    public GetPsychologistProfileValidator()
    {
        RuleFor(query => query.PsychologistId).NotEmpty();
    }
}
