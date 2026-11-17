using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IPatientRecordRepository
{
    Task<PatientRecord?> GetByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the patient and user details, origin appointment, diagnoses with
    /// catalog entries, and prescriptions with medication details.
    /// </summary>
    Task<PatientRecord?> GetByPatientIdWithDetailsAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<PatientRecord?> GetByRecordIdForTenantAsync(
        Guid recordId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>Atomically inserts only if the patient has no other record.</summary>
    Task<bool> AddIfPatientHasNoRecordAsync(
        PatientRecord record,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(PatientRecord record, CancellationToken cancellationToken = default);
}
