namespace PsyLink.API.DTOs;

/// <summary>Medication catalog entry returned by the API.</summary>
public sealed record MedicationResponseDto(
    Guid MedicationId,
    string Name,
    int Content,
    string Presentation,
    LaboratoryResponseDto? Laboratory,
    IReadOnlyList<IngredientResponseDto> Ingredients);
