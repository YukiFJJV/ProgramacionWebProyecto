using PsyLink.Domain.Common;

namespace PsyLink.Domain.Clinical;

/// <summary>Expediente clínico del paciente (uno por paciente).</summary>
public class PatientRecord : ITenantScoped
{
    public Guid RecordId { get; set; }
    public Guid PatientId { get; set; }
    public Guid TenantId { get; set; }

    /// <summary>Cita que originó el expediente (primera cita).</summary>
    public Guid? AppointmentId { get; set; }

    /// <summary>Nivel de atención, de 0 a 3. Solo se modifica con <see cref="UpdateRiskScore"/>.</summary>
    public int RiskScore { get; private set; }

    public Patient Patient { get; set; } = null!;
    public Appointment? Appointment { get; set; }

    public ICollection<RecordDiagnosis> Diagnoses { get; set; } = new List<RecordDiagnosis>();
    public ICollection<Pharmacy.Prescription> Prescriptions { get; set; } = new List<Pharmacy.Prescription>();

    public void UpdateRiskScore(int riskScore)
    {
        if (!RiskLevel.IsValid(riskScore))
            throw new DomainException("El nivel de riesgo debe estar entre 0 y 3.");

        RiskScore = riskScore;
    }
}
