using SaludMental.Domain.Clinical;

namespace SaludMental.Domain.Pharmacy;

/// <summary>
/// Medicamento sugerido para un diagnóstico (HU-10). Tabla nueva: no existe en el ER original.
/// Clave compuesta (DiagnosisId, MedicationId).
/// </summary>
public class DiagnosisMedication
{
    public Guid DiagnosisId { get; set; }
    public Guid MedicationId { get; set; }

    public DiagnosisCatalog Diagnosis { get; set; } = null!;
    public Medication Medication { get; set; } = null!;
}
