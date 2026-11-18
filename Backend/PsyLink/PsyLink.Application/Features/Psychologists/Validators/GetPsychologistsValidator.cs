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
        RuleFor(query => query.SortBy)
            .Must(sortBy => sortBy is not null &&
                new[] { "firstName", "lastName", "licenseNumber", "specialtyName" }
                    .Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("El campo de ordenamiento no está permitido.");
        RuleFor(query => query.SortDirection)
            .Must(direction => direction is not null &&
                (direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                 direction.Equals("desc", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("La dirección de ordenamiento debe ser asc o desc.");
    }
}
