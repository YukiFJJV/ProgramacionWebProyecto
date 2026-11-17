using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Pharmacy;

public sealed class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Prescription?> GetByIdForRecordAsync(
        Guid prescriptionId,
        Guid recordId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Prescriptions
            .Include(p => p.Medication)
                .ThenInclude(m => m.Laboratory)
            .Include(p => p.Medication)
                .ThenInclude(m => m.Ingredients)
                    .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(
                p => p.PrescriptionId == prescriptionId &&
                     p.RecordId == recordId &&
                     p.Record.TenantId == tenantId,
                cancellationToken);
    }

    public async Task AddAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default)
    {
        await _context.Prescriptions.AddAsync(
            prescription,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default)
    {
        _context.Prescriptions.Update(prescription);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default)
    {
        _context.Prescriptions.Remove(prescription);
        await _context.SaveChangesAsync(cancellationToken);
    }
}