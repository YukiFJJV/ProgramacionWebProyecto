using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Pharmacy;

public sealed class MedicationRepository : IMedicationRepository
{
    private readonly ApplicationDbContext _context;

    public MedicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Medication?> GetByIdAsync(
        Guid medicationId,
        CancellationToken cancellationToken = default)
    {
        return _context.Medications
            .AsNoTracking()
            .Include(m => m.Laboratory)
                .ThenInclude(l => l!.Phone)
            .Include(m => m.Ingredients)
                .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(
                m => m.MedicationId == medicationId,
                cancellationToken);
    }
}