namespace PsyLink.Application.Features.Tenants.DTOs;

public sealed record TenantQueryDto
{
    public string? Search { get; init; }
    public bool? Active { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "name";
    public string SortDirection { get; init; } = "asc";
}
