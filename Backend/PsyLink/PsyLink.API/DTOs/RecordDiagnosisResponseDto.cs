namespace PsyLink.API.DTOs;

/// <summary>Diagnosis associated with a clinical record.</summary>
public sealed record RecordDiagnosisResponseDto(
    Guid RecordId,
    DiagnosisDetailResponseDto Diagnosis,
    string? Notes);
