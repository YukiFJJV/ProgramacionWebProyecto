namespace PsyLink.API.DTOs;

/// <summary>A page of results with paging metadata.</summary>
public sealed record PageResponseDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPrevious,
    bool HasNext);
