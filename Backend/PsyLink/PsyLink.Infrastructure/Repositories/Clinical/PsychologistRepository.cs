using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PsychologistRepository : IPsychologistRepository
{
    private readonly ApplicationDbContext _context;

    public PsychologistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Psychologist> ProfileQuery()
    {
        return _context.Psychologists
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .Include(p => p.Specialty);
    }

    public Task<Psychologist?> GetByIdForTenantAsync(
        Guid psychologistId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return ProfileQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.PsychologistId == psychologistId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<Psychologist?> GetByUserIdForTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return ProfileQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.UserId == userId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> ExistsByLicenseNumberAsync(
        string licenseNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedLicense = licenseNumber.Trim();

        return _context.Psychologists
            .AsNoTracking()
            .AnyAsync(
                p => p.LicenseNumber == normalizedLicense,
                cancellationToken);
    }

    public async Task AddWithUserAsync(
        Psychologist psychologist,
        CancellationToken cancellationToken = default)
    {
        await _context.Psychologists.AddAsync(
            psychologist,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(
        IReadOnlyList<Psychologist> Psychologists,
        int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? specialtyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ProfileQuery()
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (specialtyId.HasValue)
        {
            query = query.Where(
                p => p.SpecialtyId == specialtyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(p =>
                p.User.FirstName.ToLower().Contains(term) ||
                p.User.LastName.ToLower().Contains(term) ||
                p.User.Email.ToLower().Contains(term) ||
                p.LicenseNumber.ToLower().Contains(term) ||
                p.Specialty.SpecialtyName.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var psychologists = await query
            .OrderBy(p => p.User.LastName)
            .ThenBy(p => p.User.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (psychologists, totalCount);
    }
}