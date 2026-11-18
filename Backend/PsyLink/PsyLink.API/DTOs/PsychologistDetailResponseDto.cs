namespace PsyLink.API.DTOs;

/// <summary>Detailed professional profile returned after psychologist registration.</summary>
public sealed record PsychologistDetailResponseDto(
    Guid PsychologistId,
    UserResponseDto User,
    SpecialtyResponseDto Specialty,
    string LicenseNumber,
    IReadOnlyList<ScheduleResponseDto> Schedules);
