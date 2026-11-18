using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Queries;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class GetDiagnosesValidator : AbstractValidator<GetDiagnosesQuery>
{
    public GetDiagnosesValidator()
    {
        RuleFor(request => request.Filter).NotNull();
        When(request => request.Filter is not null, () =>
        {
            RuleFor(request => request.Filter.Search).MaximumLength(200);
            RuleFor(request => request.Filter.Page).GreaterThan(0);
            RuleFor(request => request.Filter.PageSize).InclusiveBetween(1, 100);
            RuleFor(request => request.Filter.SortDirection)
                .Must(direction => direction is not null &&
                    (direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                     direction.Equals("desc", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("La dirección de ordenamiento debe ser asc o desc.");
        });
    }
}
