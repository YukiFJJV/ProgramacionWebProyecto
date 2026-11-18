namespace PsyLink.Application.Features.Users.DTOs;

public sealed record UserQueryDto(
    string? Search,
    Guid? RoleId,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "firstName",
    string SortDirection = "asc");
