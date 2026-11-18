using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IPatientDiagnosisRepository
{
    Task<IReadOnlyList<DiagnosisStatus>> GetStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientDiagnosis>> GetByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<PatientDiagnosis?> GetForUpdateAsync(
        Guid patientId,
        Guid diagnosisId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<DiagnosisCatalog?> GetActiveDiagnosisByIdAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default);

    Task<DiagnosisStatus?> GetStatusByIdAsync(
        Guid statusId,
        CancellationToken cancellationToken = default);

    Task SaveUpsertAsync(
        PatientDiagnosis patientDiagnosis,
        bool isNew,
        CancellationToken cancellationToken = default);
}
