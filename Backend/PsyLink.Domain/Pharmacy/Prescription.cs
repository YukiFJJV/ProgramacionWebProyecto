using PsyLink.Domain.Clinical;

namespace PsyLink.Domain.Pharmacy;

public class Prescription
{
    /// <summary>En el ER la PK se llama "prescription"; aquí se normaliza a PrescriptionId.</summary>
    public Guid PrescriptionId { get; set; }
    public Guid RecordId { get; set; }
    public Guid MedicationId { get; set; }

    /// <summary>Dosis e instrucciones, en texto estructurado.</summary>
    public string? Dosage { get; set; }

    public int? Quantity { get; set; }

    public PatientRecord Record { get; set; } = null!;
    public Medication Medication { get; set; } = null!;
}
