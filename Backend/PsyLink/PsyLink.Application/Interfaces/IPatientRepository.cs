using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    /// <summary>Loads the patient with its user, role, and phone relationships.</summary>
    Task<Patient?> GetByIdForTenantAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByIdForPsychologistAsync(
        Guid patientId,
        Guid tenantId,
        Guid psychologistUserId,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByIdForPsychologistForUpdateAsync(
        Guid patientId,
        Guid tenantId,
        Guid psychologistUserId,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByUserIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);

    Task AddWithUserAsync(
        Patient patient,
        User user,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Patient> Patients, int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? psychologistId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        Guid? psychologistUserId = null,
        CancellationToken cancellationToken = default);
}
