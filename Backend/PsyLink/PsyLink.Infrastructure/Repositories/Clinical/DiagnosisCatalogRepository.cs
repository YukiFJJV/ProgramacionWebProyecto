using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class DiagnosisCatalogRepository : IDiagnosisCatalogRepository
{
    private readonly ApplicationDbContext _context;

    public DiagnosisCatalogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<DiagnosisCatalog?> GetByIdAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default)
    {
        return _context.DiagnosisCatalog
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.DiagnosisId == diagnosisId,
                cancellationToken);
    }
}