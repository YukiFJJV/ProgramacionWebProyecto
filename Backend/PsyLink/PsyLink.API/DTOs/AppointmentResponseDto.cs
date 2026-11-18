namespace PsyLink.API.DTOs;

/// <summary>Appointment summary returned by list and rescheduling operations.</summary>
public sealed record AppointmentResponseDto(
    Guid AppointmentId,
    Guid PatientId,
    string? PatientName,
    Guid PsychologistId,
    string? PsychologistName,
    DateOnly DateAt,
    TimeOnly StartsAt,
    TimeOnly EndsAt);
