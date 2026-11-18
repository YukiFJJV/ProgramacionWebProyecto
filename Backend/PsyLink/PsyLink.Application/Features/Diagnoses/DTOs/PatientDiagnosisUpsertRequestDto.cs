namespace PsyLink.Application.Features.Diagnoses.DTOs;

public sealed record PatientDiagnosisUpsertRequestDto(
    Guid StatusId,
    string? Notes);
