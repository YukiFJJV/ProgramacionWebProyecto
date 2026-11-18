namespace PsyLink.API.DTOs;

/// <summary>Search, filter, sort, and pagination options for tenant users.</summary>
public sealed record UserQueryDto
{
    public string? Search { get; init; }
    public Guid? RoleId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "firstName";
    public string SortDirection { get; init; } = "asc";
}
