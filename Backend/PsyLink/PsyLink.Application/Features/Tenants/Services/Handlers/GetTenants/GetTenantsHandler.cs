using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Tenants.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Tenants.Services.Handlers.GetTenants;

public sealed class GetTenantsHandler(
    ITenantRepository tenantRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetTenantsQuery, TenantPageDto>
{
    public async Task<TenantPageDto> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.SuperAdmin)
        {
            throw new UnauthorizedAccessException("La operación requiere el rol SUPER_ADMIN.");
        }

        var (tenants, totalCount) = await tenantRepository.SearchAsync(
            request.TenantId,
            request.Name,
            request.Rtn,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new TenantPageDto
        {
            Items = tenants.Select(tenant => new TenantDto
            {
                TenantId = tenant.TenantId,
                Name = tenant.Name,
                Email = tenant.Email,
                Phone = tenant.Phone?.Number,
                Rtn = tenant.Rtn,
                Active = tenant.Active
            }).ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
