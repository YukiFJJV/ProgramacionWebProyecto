using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IDiagnosisCatalogRepository
{
    Task<DiagnosisCatalog?> GetByIdAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<DiagnosisCatalog> Diagnoses, int TotalCount)> SearchActiveAsync(
        string? search,
        int page,
        int pageSize,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        DiagnosisCatalog diagnosis,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        DiagnosisCatalog diagnosis,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default);
}
