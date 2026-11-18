namespace PsyLink.API.DTOs;

/// <summary>Medication prescription associated with a clinical record.</summary>
public sealed record PrescriptionResponseDto(
    Guid PrescriptionId,
    Guid RecordId,
    MedicationResponseDto Medication,
    string? Dosage,
    int? Quantity);
