using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.GetAssignableRoles;

public sealed class GetAssignableRolesHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAssignableRolesQuery, IReadOnlyList<RoleResponseDto>>
{
    public async Task<IReadOnlyList<RoleResponseDto>> Handle(
        GetAssignableRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador de empresa autenticado.");
        }

        var roles = await userRepository.GetAssignableRolesAsync(cancellationToken);
        return roles
            .Where(role => role.Name != RoleNames.SuperAdmin)
            .Select(role => new RoleResponseDto
            {
                RoleId = role.RoleId,
                Name = role.Name
            })
            .ToList();
    }
}
