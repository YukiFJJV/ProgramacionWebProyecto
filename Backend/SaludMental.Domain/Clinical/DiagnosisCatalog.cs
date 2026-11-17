using SaludMental.Domain.Pharmacy;

namespace SaludMental.Domain.Clinical;

public class DiagnosisCatalog
{
    public Guid DiagnosisId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Columna nueva: borrado lógico (HU-10 impide el borrado físico si hay referencias).</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Medicamentos sugeridos para este diagnóstico (HU-10).</summary>
    public ICollection<DiagnosisMedication> SuggestedMedications { get; set; } = new List<DiagnosisMedication>();
}
