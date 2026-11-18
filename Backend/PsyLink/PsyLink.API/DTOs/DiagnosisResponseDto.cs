namespace PsyLink.API.DTOs;

/// <summary>Diagnosis identifier and name returned in catalog listings.</summary>
public sealed record DiagnosisResponseDto(Guid DiagnosisId, string Name);
