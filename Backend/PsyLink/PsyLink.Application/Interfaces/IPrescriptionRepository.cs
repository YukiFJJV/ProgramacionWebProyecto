using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Interfaces;

public interface IPrescriptionRepository
{
    /// <summary>Loads the prescription and its medication details.</summary>
    Task<Prescription?> GetByIdForRecordAsync(
        Guid prescriptionId,
        Guid recordId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default);

    Task UpdateAsync(Prescription prescription, CancellationToken cancellationToken = default);

    Task DeleteAsync(Prescription prescription, CancellationToken cancellationToken = default);
}
