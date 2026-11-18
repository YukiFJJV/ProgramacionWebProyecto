using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Features.Medications.Services;

internal static class MedicationDtoMapper
{
    public static MedicationResponseDto ToResponse(Medication medication) =>
        new()
        {
            MedicationId = medication.MedicationId,
            Name = medication.Name,
            Content = medication.Content,
            Presentation = medication.Presentation,
            Laboratory = medication.Laboratory is null
                ? null
                : new LaboratoryResponseDto
                {
                    LaboratoryId = medication.Laboratory.LaboratoryId,
                    Name = medication.Laboratory.Name,
                    Phone = medication.Laboratory.Phone is null
                        ? null
                        : new PhoneNumberResponseDto
                        {
                            PhoneId = medication.Laboratory.Phone.PhoneId,
                            Number = medication.Laboratory.Phone.Number
                        }
                },
            Ingredients = medication.Ingredients
                .Select(medicationIngredient => new IngredientResponseDto
                {
                    IngredientId = medicationIngredient.Ingredient.IngredientId,
                    Name = medicationIngredient.Ingredient.Name
                })
                .ToList()
        };
}
