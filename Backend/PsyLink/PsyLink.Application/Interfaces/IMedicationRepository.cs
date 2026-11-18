using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Interfaces;

public interface IMedicationRepository
{
    /// <summary>Loads the medication, laboratory/phone, and ingredient details.</summary>
    Task<Medication?> GetByIdAsync(
        Guid medicationId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Medication> Items, int TotalCount)> SearchAsync(
        string? search,
        Guid? laboratoryId,
        Guid? ingredientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> LaboratoryExistsAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActiveIngredient>> GetIngredientsByIdsAsync(
        IReadOnlyCollection<Guid> ingredientIds,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Medication medication,
        CancellationToken cancellationToken = default);
}
