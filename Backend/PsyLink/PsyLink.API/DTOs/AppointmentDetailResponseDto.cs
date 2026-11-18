namespace PsyLink.API.DTOs;

/// <summary>Detailed appointment data with its patient and psychologist.</summary>
public sealed record AppointmentDetailResponseDto(
    Guid AppointmentId,
    PatientResponseDto Patient,
    PsychologistSummaryResponseDto Psychologist,
    DateOnly DateAt,
    TimeOnly StartsAt,
    TimeOnly EndsAt);
