using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.GetUsers;

public sealed class GetUsersHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUsersQuery, UserPageDto>
{
    public async Task<UserPageDto> Handle(
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

        return new UserPageDto
        {
            Items = users.Select(CreateUserHandler.ToResponse).ToList(),
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
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
