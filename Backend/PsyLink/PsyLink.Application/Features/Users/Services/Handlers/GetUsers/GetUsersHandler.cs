using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.GetUsers;

public sealed class GetUsersHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUsersQuery, PageResponseDto<UserResponseDto>>
{
    public async Task<PageResponseDto<UserResponseDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureCompanyAdmin(currentUser);
        var (users, totalCount) = await userRepository.SearchByTenantAsync(
            tenantId,
            request.Search,
            request.RoleId,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = users
            .Select(PsyLink.Application.Features.Users.Shared.UserDtoMapper.ToResponse)
            .ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        return new PageResponseDto<UserResponseDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages,
            request.Page > 1,
            request.Page < totalPages);
    }

    internal static Guid EnsureCompanyAdmin(ICurrentUserService currentUser)
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }
}
