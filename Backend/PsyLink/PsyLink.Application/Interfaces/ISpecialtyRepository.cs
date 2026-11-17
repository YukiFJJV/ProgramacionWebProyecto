using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface ISpecialtyRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingSpecialtyId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Specialty specialty, CancellationToken cancellationToken = default);

    Task<Specialty?> GetByIdAsync(Guid specialtyId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Specialty specialty, CancellationToken cancellationToken = default);
}
