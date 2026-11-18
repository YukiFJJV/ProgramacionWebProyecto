using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class RecordDiagnosisRepository : IRecordDiagnosisRepository
{
    private readonly ApplicationDbContext _context;

    public RecordDiagnosisRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<RecordDiagnosis?> GetByRecordAndDiagnosisAsync(
        Guid recordId,
        Guid diagnosisId,
        CancellationToken cancellationToken = default)
    {
        return _context.RecordDiagnoses
            .Include(rd => rd.Diagnosis)
            .FirstOrDefaultAsync(
                rd => rd.RecordId == recordId &&
                      rd.DiagnosisId == diagnosisId,
                cancellationToken);
    }

    public async Task AddAsync(
        RecordDiagnosis recordDiagnosis,
        CancellationToken cancellationToken = default)
    {
        await _context.RecordDiagnoses.AddAsync(
            recordDiagnosis,
            cancellationToken);

    }

    public async Task UpdateAsync(
        RecordDiagnosis recordDiagnosis,
        CancellationToken cancellationToken = default)
    {
        _context.RecordDiagnoses.Update(recordDiagnosis);

    }

    public async Task DeleteAsync(
        RecordDiagnosis recordDiagnosis,
        CancellationToken cancellationToken = default)
    {
        _context.RecordDiagnoses.Remove(recordDiagnosis);

    }
}