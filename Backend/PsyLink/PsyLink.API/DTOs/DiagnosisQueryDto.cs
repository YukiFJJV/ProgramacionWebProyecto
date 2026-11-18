namespace PsyLink.API.DTOs;

/// <summary>Search and pagination options for the diagnosis catalog.</summary>
public sealed record DiagnosisQueryDto
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortDirection { get; init; } = "asc";
}
