namespace PsyLink.Application.Features.Shared.DTOs;

public sealed record PageResponseDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPrevious,
    bool HasNext);
