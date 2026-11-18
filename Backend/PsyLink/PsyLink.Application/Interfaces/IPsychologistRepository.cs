using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IPsychologistRepository
{
    /// <summary>Loads the profile with its user, role, phone, and specialty relationships.</summary>
    Task<Psychologist?> GetByIdForTenantAsync(
        Guid psychologistId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Psychologist?> GetByUserIdForTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);

    Task AddWithUserAsync(Psychologist psychologist, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Psychologist> Psychologists, int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? specialtyId,
        DateOnly? availableOn,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default);
}
