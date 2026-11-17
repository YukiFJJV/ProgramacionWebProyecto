using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Patient?> GetByIdForTenantAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .FirstOrDefaultAsync(
                p => p.PatientId == patientId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<Patient?> GetByUserIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .FirstOrDefaultAsync(
                p => p.UserId == userId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .AnyAsync(
                p => p.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}