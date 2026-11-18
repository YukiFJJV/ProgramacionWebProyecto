using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Pharmacy;

public sealed class MedicationRepository : IMedicationRepository
{
    private readonly ApplicationDbContext _context;

    public MedicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Medication> DisplayQuery()
    {
        return _context.Medications
            .Include(m => m.Laboratory)
                .ThenInclude(l => l!.Phone)
            .Include(m => m.Ingredients)
                .ThenInclude(i => i.Ingredient);
    }

    public Task<Medication?> GetByIdAsync(
        Guid medicationId,
        CancellationToken cancellationToken = default)
    {
        return DisplayQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.MedicationId == medicationId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Medication> Items, int TotalCount)> SearchAsync(
        string? search,
        Guid? laboratoryId,
        Guid? ingredientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DisplayQuery()
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(medication => medication.Name.ToLower().Contains(term));
        }

        if (laboratoryId.HasValue)
        {
            query = query.Where(medication => medication.LaboratoryId == laboratoryId.Value);
        }

        if (ingredientId.HasValue)
        {
            query = query.Where(medication =>
                medication.Ingredients.Any(ingredient =>
                    ingredient.IngredientId == ingredientId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var medications = await query
            .OrderBy(medication => medication.Name)
            .ThenBy(medication => medication.MedicationId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (medications, totalCount);
    }

    public Task<bool> LaboratoryExistsAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return _context.Laboratories
            .AsNoTracking()
            .AnyAsync(laboratory => laboratory.LaboratoryId == laboratoryId, cancellationToken);
    }

    public async Task<IReadOnlyList<ActiveIngredient>> GetIngredientsByIdsAsync(
        IReadOnlyCollection<Guid> ingredientIds,
        CancellationToken cancellationToken = default)
    {
        if (ingredientIds.Count == 0)
        {
            return Array.Empty<ActiveIngredient>();
        }

        return await _context.ActiveIngredients
            .AsNoTracking()
            .Where(ingredient => ingredientIds.Contains(ingredient.IngredientId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Medication medication,
        CancellationToken cancellationToken = default)
    {
        var ingredientIds = medication.Ingredients
            .Select(medicationIngredient => medicationIngredient.IngredientId)
            .Distinct()
            .ToArray();
        var ingredients = await _context.ActiveIngredients
            .Where(ingredient => ingredientIds.Contains(ingredient.IngredientId))
            .ToDictionaryAsync(ingredient => ingredient.IngredientId, cancellationToken);
        if (ingredients.Count != ingredientIds.Length)
        {
            throw new KeyNotFoundException("Uno o más principios activos no existen.");
        }

        foreach (var medicationIngredient in medication.Ingredients)
        {
            medicationIngredient.Medication = medication;
            medicationIngredient.Ingredient = ingredients[medicationIngredient.IngredientId];
        }

        if (medication.LaboratoryId is Guid laboratoryId)
        {
            medication.Laboratory = await _context.Laboratories
                .Include(laboratory => laboratory.Phone)
                .FirstOrDefaultAsync(
                    laboratory => laboratory.LaboratoryId == laboratoryId,
                    cancellationToken)
                ?? throw new KeyNotFoundException($"No se encontró el laboratorio {laboratoryId}.");
        }

        await _context.Medications.AddAsync(medication, cancellationToken);

    }
}