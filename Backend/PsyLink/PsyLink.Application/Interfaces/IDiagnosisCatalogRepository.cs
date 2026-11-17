using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IDiagnosisCatalogRepository
{
    Task<DiagnosisCatalog?> GetByIdAsync(
        Guid diagnosisId,
        CancellationToken cancellationToken = default);
}
