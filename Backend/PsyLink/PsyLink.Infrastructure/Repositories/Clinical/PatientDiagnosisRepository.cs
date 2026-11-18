using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PatientDiagnosisRepository(ApplicationDbContext context)
    : IPatientDiagnosisRepository
{
    public async Task<IReadOnlyList<DiagnosisStatus>> GetStatusesAsync(
        CancellationToken cancellationToken = default) =>
        await context.Set<DiagnosisStatus>()
            .AsNoTracking()
            .OrderBy(status => status.Name)
            .ThenBy(status => status.StatusId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PatientDiagnosis>> GetByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await context.PatientDiagnoses
            .AsNoTracking()
            .Where(item =>
                item.PatientId == patientId &&
                item.Patient.TenantId == tenantId)
            .Include(item => item.Diagnosis)
            .Include(item => item.Status)
            .OrderBy(item => item.Diagnosis.Name)
            .ThenBy(item => item.DiagnosisId)
            .ToListAsync(cancellationToken);

    public Task<PatientDiagnosis?> GetForUpdateAsync(
        Guid patientId,
        Guid diagnosisId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        context.PatientDiagnoses
            .Include(item => item.Diagnosis)
            .Include(item => item.Status)
            .FirstOrDefaultAsync(
                item =>
                    item.PatientId == patientId &&
                    item.DiagnosisId == diagnosisId &&
                    item.Patient.TenantId == tenantId,
                cancellationToken);

    public Task<DiagnosisCatalog?> GetActiveDiagnosisByIdAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default) =>
        context.DiagnosisCatalog
            .FirstOrDefaultAsync(
                diagnosis => diagnosis.DiagnosisId == diagnosisId && diagnosis.IsActive,
                cancellationToken);

    public Task<DiagnosisStatus?> GetStatusByIdAsync(
        Guid statusId,
        CancellationToken cancellationToken = default) =>
        context.Set<DiagnosisStatus>()
            .FirstOrDefaultAsync(status => status.StatusId == statusId, cancellationToken);

    public async Task SaveUpsertAsync(
        PatientDiagnosis patientDiagnosis,
        bool isNew,
        CancellationToken cancellationToken = default)
    {
        if (isNew)
        {
            await context.PatientDiagnoses.AddAsync(patientDiagnosis, cancellationToken);
        }

    }
}
