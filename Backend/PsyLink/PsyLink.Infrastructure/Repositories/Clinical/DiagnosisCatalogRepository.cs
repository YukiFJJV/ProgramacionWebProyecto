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

    public async Task<(IReadOnlyList<DiagnosisCatalog> Diagnoses, int TotalCount)> SearchActiveAsync(
        string? search,
        int page,
        int pageSize,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        var query = _context.DiagnosisCatalog
            .AsNoTracking()
            .Where(diagnosis => diagnosis.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(diagnosis => diagnosis.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var orderedQuery = descending
            ? query.OrderByDescending(diagnosis => diagnosis.Name)
                .ThenBy(diagnosis => diagnosis.DiagnosisId)
            : query.OrderBy(diagnosis => diagnosis.Name)
                .ThenBy(diagnosis => diagnosis.DiagnosisId);
        var diagnoses = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (diagnoses, totalCount);
    }

    public async Task AddAsync(
        DiagnosisCatalog diagnosis,
        CancellationToken cancellationToken = default)
    {
        await _context.DiagnosisCatalog.AddAsync(diagnosis, cancellationToken);

    }

    public async Task UpdateAsync(
        DiagnosisCatalog diagnosis,
        CancellationToken cancellationToken = default)
    {
        _context.DiagnosisCatalog.Update(diagnosis);

    }

    public async Task<bool> DeleteAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default)
    {
        var diagnosis = await _context.DiagnosisCatalog
            .FirstOrDefaultAsync(
                item => item.DiagnosisId == diagnosisId && item.IsActive,
                cancellationToken);
        if (diagnosis is null)
        {
            return false;
        }

        var hasReferences =
            await _context.RecordDiagnoses.AnyAsync(
                item => item.DiagnosisId == diagnosisId,
                cancellationToken) ||
            await _context.PatientDiagnoses.AnyAsync(
                item => item.DiagnosisId == diagnosisId,
                cancellationToken) ||
            await _context.DiagnosisMedications.AnyAsync(
                item => item.DiagnosisId == diagnosisId,
                cancellationToken);

        if (hasReferences)
        {
            diagnosis.IsActive = false;
        }
        else
        {
            _context.DiagnosisCatalog.Remove(diagnosis);
        }

        return true;
    }
}