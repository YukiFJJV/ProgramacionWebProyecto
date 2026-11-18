namespace PsyLink.API.DTOs;

/// <summary>Sets the current status for a patient's diagnosis.</summary>
public sealed record PatientDiagnosisUpsertRequestDto(Guid StatusId, string? Notes = null);
