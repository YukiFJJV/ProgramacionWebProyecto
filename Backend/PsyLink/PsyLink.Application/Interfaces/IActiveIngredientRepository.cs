using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Interfaces;

public interface IActiveIngredientRepository
{
    Task<IReadOnlyList<ActiveIngredient>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
