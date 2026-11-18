namespace PsyLink.API.DTOs;

/// <summary>Diagnosis and current status associated with a patient.</summary>
public sealed record PatientDiagnosisResponseDto(
    Guid PatientId,
    DiagnosisDetailResponseDto Diagnosis,
    DiagnosisStatusResponseDto Status,
    DateTime UpdatedAt);
