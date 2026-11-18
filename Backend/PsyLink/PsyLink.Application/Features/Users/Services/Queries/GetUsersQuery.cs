using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Users.Services.Queries;

public record GetUsersQuery(
    string? Search = null,
    Guid? RoleId = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "firstName",
    string SortDirection = "asc") : IQuery<PageResponseDto<UserResponseDto>>;
