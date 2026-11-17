namespace SaludMental.Domain.Pharmacy;

/// <summary>Relación medicamento-principio activo. Clave compuesta (MedicationId, IngredientId).</summary>
public class MedicationIngredient
{
    public Guid MedicationId { get; set; }
    public Guid IngredientId { get; set; }

    public Medication Medication { get; set; } = null!;
    public ActiveIngredient Ingredient { get; set; } = null!;
}
