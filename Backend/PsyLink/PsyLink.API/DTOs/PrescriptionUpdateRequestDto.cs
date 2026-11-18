namespace PsyLink.API.DTOs;

/// <summary>Optional fields that may be changed on a clinical-record prescription.</summary>
public sealed record PrescriptionUpdateRequestDto
{
    public Guid? MedicationId { get; init; }
    public string? Dosage { get; init; }
    public int? Quantity { get; init; }
}
