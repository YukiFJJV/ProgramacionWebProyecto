namespace PsyLink.API.DTOs;

/// <summary>Search, specialty, availability, sort, and pagination options for psychologists.</summary>
public sealed record PsychologistQueryDto
{
    public string? Search { get; init; }
    public Guid? SpecialtyId { get; init; }
    public DateOnly? AvailableOn { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "lastName";
    public string SortDirection { get; init; } = "asc";
}
