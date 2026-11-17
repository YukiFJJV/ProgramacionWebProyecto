namespace PsyLink.Domain.Clinical;

/// <summary>Diagnóstico asociado a un expediente. Clave compuesta (RecordId, DiagnosisId).</summary>
public class RecordDiagnosis
{
    public Guid RecordId { get; set; }
    public Guid DiagnosisId { get; set; }
    public string? Notes { get; set; }

    public PatientRecord Record { get; set; } = null!;
    public DiagnosisCatalog Diagnosis { get; set; } = null!;
}
