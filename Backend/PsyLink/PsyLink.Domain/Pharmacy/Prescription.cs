using PsyLink.Domain.Clinical;
using PsyLink.Domain.Common;

namespace PsyLink.Domain.Pharmacy;

public class Prescription
{
    private Prescription() { }

    public static Prescription Issue(
        Guid recordId,
        Guid medicationId,
        string? dosage,
        int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad prescrita debe ser mayor que cero.");

        return new Prescription
        {
            PrescriptionId = Guid.NewGuid(),
            RecordId = recordId,
            MedicationId = medicationId,
            Dosage = dosage,
            Quantity = quantity
        };
    }

    /// <summary>En el ER la PK se llama "prescription"; aquí se normaliza a PrescriptionId.</summary>
    public Guid PrescriptionId { get; set; }
    public Guid RecordId { get; set; }
    public Guid MedicationId { get; set; }

    /// <summary>Dosis e instrucciones, en texto estructurado.</summary>
    public string? Dosage { get; set; }

    public int? Quantity { get; set; }

    public PatientRecord Record { get; set; } = null!;
    public Medication Medication { get; set; } = null!;

    public void Update(string? dosage, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad prescrita debe ser mayor que cero.");

        Dosage = dosage;
        Quantity = quantity;
    }

    public void ChangeMedication(Guid medicationId)
    {
        if (medicationId == Guid.Empty)
            throw new DomainException("El medicamento es obligatorio.");

        MedicationId = medicationId;
    }
}
