using FluentValidation;
using PsyLink.Application.Features.Psychologists.Services.Queries;

namespace PsyLink.Application.Features.Psychologists.Validators;

public sealed class GetPsychologistsValidator : AbstractValidator<GetPsychologistsQuery>
{
    public GetPsychologistsValidator()
    {
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.Page).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
