
using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Identity;

public sealed class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingTenantId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants
            .AsNoTracking()
            .Where(t => t.Name.ToLower() == name.Trim().ToLower());

        if (excludingTenantId.HasValue)
        {
            query = query.Where(t => t.TenantId != excludingTenantId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsByRtnAsync(
        string rtn,
        Guid? excludingTenantId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants
            .AsNoTracking()
            .Where(t => t.Rtn != null && t.Rtn == rtn.Trim());

        if (excludingTenantId.HasValue)
        {
            query = query.Where(t => t.TenantId != excludingTenantId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);

    }

    public Task<Tenant?> GetByIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Tenants
            .Include(t => t.Phone)
            .FirstOrDefaultAsync(
                t => t.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Tenant> Tenants, int TotalCount)> SearchAsync(
        Guid? tenantId,
        string? name,
        string? rtn,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants
            .AsNoTracking()
            .Include(t => t.Phone)
            .AsQueryable();

        if (tenantId.HasValue)
        {
            query = query.Where(t => t.TenantId == tenantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var searchName = name.Trim().ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(searchName));
        }

        if (!string.IsNullOrWhiteSpace(rtn))
        {
            var searchRtn = rtn.Trim();
            query = query.Where(t =>
                t.Rtn != null && t.Rtn.Contains(searchRtn));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tenants = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (tenants, totalCount);
    }

    public async Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);

    }
}
