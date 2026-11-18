namespace PsyLink.Application.Features.Patients.DTOs;

public sealed record PatientDiagnosisUpsertRequestDto(
    Guid StatusId,
    string? Notes);
