using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class SpecialtyRepository : ISpecialtyRepository
{
    private readonly ApplicationDbContext _context;

    public SpecialtyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingSpecialtyId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLower();

        var query = _context.Specialties
            .AsNoTracking()
            .Where(s => s.SpecialtyName.ToLower() == normalizedName);

        if (excludingSpecialtyId.HasValue)
        {
            query = query.Where(
                s => s.SpecialtyId != excludingSpecialtyId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Specialty specialty,
        CancellationToken cancellationToken = default)
    {
        await _context.Specialties.AddAsync(
            specialty,
            cancellationToken);

    }

    public Task<Specialty?> GetByIdAsync(
        Guid specialtyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Specialties
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.SpecialtyId == specialtyId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Specialty>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Specialties
            .AsNoTracking()
            .OrderBy(s => s.SpecialtyName)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Specialty specialty,
        CancellationToken cancellationToken = default)
    {
        _context.Specialties.Update(specialty);

    }
}