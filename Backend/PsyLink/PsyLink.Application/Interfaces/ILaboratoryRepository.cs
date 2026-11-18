using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Interfaces;

public interface ILaboratoryRepository
{
    Task<IReadOnlyList<Laboratory>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
