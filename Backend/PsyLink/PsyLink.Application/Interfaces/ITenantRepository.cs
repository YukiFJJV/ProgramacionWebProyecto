using PsyLink.Domain.Identity;

namespace PsyLink.Application.Interfaces;

public interface ITenantRepository
{
    Task<bool> ExistsByNameAsync(string name, Guid? excludingTenantId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByRtnAsync(string rtn, Guid? excludingTenantId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    Task<Tenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<Tenant?> GetFirstActiveAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Tenant> Tenants, int TotalCount)> SearchAsync(
        Guid? tenantId,
        string? name,
        string? rtn,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
