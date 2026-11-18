namespace PsyLink.API.DTOs;

/// <summary>Payload for creating a medication and linking existing active ingredients.</summary>
public sealed record MedicationCreateRequestDto
{
    public Guid? LaboratoryId { get; init; }
    public required string Name { get; init; }
    public int Content { get; init; }
    public required string Presentation { get; init; }
    public List<Guid>? IngredientIds { get; init; } = new();
}
