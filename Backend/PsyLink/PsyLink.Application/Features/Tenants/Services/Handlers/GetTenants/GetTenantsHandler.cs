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
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol SUPER_ADMIN.");
        }

        const int fetchPageSize = 100;
        var allTenants = new List<TenantDto>();
        var fetchPage = 1;
        int totalCount;
        do
        {
            var (tenants, count) = await tenantRepository.SearchAsync(
                null,
                null,
                null,
                fetchPage,
                fetchPageSize,
                cancellationToken);
            totalCount = count;
            allTenants.AddRange(tenants.Select(tenant => new TenantDto
            {
                TenantId = tenant.TenantId,
                Name = tenant.Name,
                Email = tenant.Email,
                Phone = tenant.Phone?.Number,
                PhoneId = tenant.Phone?.PhoneId,
                Rtn = tenant.Rtn,
                Active = tenant.Active
            }));
            fetchPage++;
        }
        while (allTenants.Count < totalCount);

        IEnumerable<TenantDto> filtered = allTenants;
        if (request.Active.HasValue)
        {
            filtered = filtered.Where(tenant => tenant.Active == request.Active.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            if (Guid.TryParse(search, out var tenantId))
            {
                filtered = filtered.Where(tenant => tenant.TenantId == tenantId);
            }
            else
            {
                filtered = filtered.Where(tenant =>
                    tenant.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (tenant.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (tenant.Rtn?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            }
        }

        var ordered = OrderTenants(filtered, request.SortBy, request.SortDirection);
        var resultItems = ordered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        var filteredCount = ordered.Count();

        return new TenantPageDto
        {
            Items = resultItems,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = filteredCount
        };
    }

    private static IOrderedEnumerable<TenantDto> OrderTenants(
        IEnumerable<TenantDto> tenants,
        string sortBy,
        string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var ordered = sortBy.ToLowerInvariant() switch
        {
            "email" => descending
                ? tenants.OrderByDescending(tenant => tenant.Email, StringComparer.OrdinalIgnoreCase)
                : tenants.OrderBy(tenant => tenant.Email, StringComparer.OrdinalIgnoreCase),
            "rtn" => descending
                ? tenants.OrderByDescending(tenant => tenant.Rtn, StringComparer.OrdinalIgnoreCase)
                : tenants.OrderBy(tenant => tenant.Rtn, StringComparer.OrdinalIgnoreCase),
            "active" => descending
                ? tenants.OrderByDescending(tenant => tenant.Active)
                : tenants.OrderBy(tenant => tenant.Active),
            "tenantid" => descending
                ? tenants.OrderByDescending(tenant => tenant.TenantId)
                : tenants.OrderBy(tenant => tenant.TenantId),
            _ => descending
                ? tenants.OrderByDescending(tenant => tenant.Name, StringComparer.OrdinalIgnoreCase)
                : tenants.OrderBy(tenant => tenant.Name, StringComparer.OrdinalIgnoreCase)
        };
        return descending
            ? ordered.ThenByDescending(tenant => tenant.TenantId)
            : ordered.ThenBy(tenant => tenant.TenantId);
    }
}
