namespace PsyLink.API.DTOs;

/// <summary>Diagnosis status returned with a patient's diagnosis.</summary>
public sealed record DiagnosisStatusResponseDto(Guid StatusId, string Name);
