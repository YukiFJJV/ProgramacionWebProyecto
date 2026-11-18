using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Pharmacy;

public sealed class LaboratoryRepository(ApplicationDbContext context) : ILaboratoryRepository
{
    public async Task<IReadOnlyList<Laboratory>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Laboratories
            .AsNoTracking()
            .Include(laboratory => laboratory.Phone)
            .OrderBy(laboratory => laboratory.Name)
            .ThenBy(laboratory => laboratory.LaboratoryId)
            .ToListAsync(cancellationToken);
    }
}
