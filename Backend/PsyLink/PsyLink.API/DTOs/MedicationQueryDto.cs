namespace PsyLink.API.DTOs;

/// <summary>Search filters and pagination for the medication catalog.</summary>
public sealed record MedicationQueryDto
{
    public string? Search { get; init; }
    public Guid? LaboratoryId { get; init; }
    public Guid? IngredientId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
