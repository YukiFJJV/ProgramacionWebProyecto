namespace SaludMental.Domain.Clinical;

/// <summary>Relación paciente-diagnóstico. Clave compuesta (DiagnosisId, PatientId).</summary>
public class PatientDiagnosis
{
    public Guid DiagnosisId { get; set; }
    public Guid PatientId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DiagnosisCatalog Diagnosis { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public DiagnosisStatus Status { get; set; } = null!;
}
