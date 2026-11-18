namespace PsyLink.API.DTOs;

/// <summary>Clinical record details returned after creation.</summary>
public sealed record RecordDetailResponseDto(
    Guid RecordId,
    PatientResponseDto Patient,
    AppointmentResponseDto? Appointment,
    int RiskScore,
    IReadOnlyList<RecordDiagnosisResponseDto> Diagnoses,
    IReadOnlyList<PrescriptionResponseDto> Prescriptions);
