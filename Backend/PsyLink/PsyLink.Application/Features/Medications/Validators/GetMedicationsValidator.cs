using FluentValidation;
using PsyLink.Application.Features.Medications.Services.Queries;

namespace PsyLink.Application.Features.Medications.Validators;

public sealed class GetMedicationsValidator : AbstractValidator<GetMedicationsQuery>
{
    public GetMedicationsValidator()
    {
        RuleFor(request => request.Filter).NotNull();
        When(request => request.Filter is not null, () =>
        {
            RuleFor(request => request.Filter.Search).MaximumLength(200);
            RuleFor(request => request.Filter.Page).GreaterThan(0);
            RuleFor(request => request.Filter.PageSize).InclusiveBetween(1, 100);
            RuleFor(request => request.Filter.LaboratoryId)
                .Must(laboratoryId => laboratoryId is null || laboratoryId != Guid.Empty)
                .WithMessage("El identificador del laboratorio no puede estar vacío.");
            RuleFor(request => request.Filter.IngredientId)
                .Must(ingredientId => ingredientId is null || ingredientId != Guid.Empty)
                .WithMessage("El identificador del principio activo no puede estar vacío.");
        });
    }
}
