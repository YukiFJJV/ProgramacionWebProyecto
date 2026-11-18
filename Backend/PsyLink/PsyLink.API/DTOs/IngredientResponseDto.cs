namespace PsyLink.API.DTOs;

/// <summary>Active ingredient included in a medication response.</summary>
public sealed record IngredientResponseDto(Guid IngredientId, string Name);
