using FluentValidation;
using PsyLink.Application.Features.Tenants.Services.Queries;

namespace PsyLink.Application.Features.Tenants.Validators;

public sealed class GetTenantsValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsValidator()
    {
        RuleFor(query => query.PageNumber).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Name).MaximumLength(100);
        RuleFor(query => query.Rtn).MaximumLength(50);
    }
}
