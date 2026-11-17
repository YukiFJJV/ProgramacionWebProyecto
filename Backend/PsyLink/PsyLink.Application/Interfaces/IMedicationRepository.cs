using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Interfaces;

public interface IMedicationRepository
{
    /// <summary>Loads the medication, laboratory/phone, and ingredient details.</summary>
    Task<Medication?> GetByIdAsync(
        Guid medicationId,
        CancellationToken cancellationToken = default);
}
