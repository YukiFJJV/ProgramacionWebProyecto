using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IPatientRepository
{
    /// <summary>Loads the patient with its user, role, and phone relationships.</summary>
    Task<Patient?> GetByIdForTenantAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByUserIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
}
