using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Pharmacy;

public sealed class ActiveIngredientRepository(ApplicationDbContext context)
    : IActiveIngredientRepository
{
    public async Task<IReadOnlyList<ActiveIngredient>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.ActiveIngredients
            .AsNoTracking()
            .OrderBy(ingredient => ingredient.Name)
            .ThenBy(ingredient => ingredient.IngredientId)
            .ToListAsync(cancellationToken);
    }
}
