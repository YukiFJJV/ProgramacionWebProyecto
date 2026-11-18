using FluentValidation;
using PsyLink.Application.Features.Medications.Services.Commands;

namespace PsyLink.Application.Features.Medications.Validators;

public sealed class CreateMedicationValidator : AbstractValidator<CreateMedicationCommand>
{
    public CreateMedicationValidator()
    {
        RuleFor(command => command.Medication).NotNull();
        When(command => command.Medication is not null, () =>
        {
            RuleFor(command => command.Medication.Name).NotEmpty();
            RuleFor(command => command.Medication.Content).GreaterThan(0);
            RuleFor(command => command.Medication.Presentation).NotEmpty();
            RuleFor(command => command.Medication.LaboratoryId)
                .Must(laboratoryId => laboratoryId is null || laboratoryId != Guid.Empty)
                .WithMessage("El identificador del laboratorio no puede estar vacío.");
            When(command => command.Medication.IngredientIds is not null, () =>
            {
                RuleForEach(command => command.Medication.IngredientIds)
                    .NotEmpty()
                    .WithMessage("El identificador del principio activo no puede estar vacío.");
                RuleFor(command => command.Medication.IngredientIds)
                    .Must(ingredientIds => ingredientIds!.Distinct().Count() == ingredientIds.Count)
                    .WithMessage("No se puede repetir un principio activo.");
            });
        });
    }
}
