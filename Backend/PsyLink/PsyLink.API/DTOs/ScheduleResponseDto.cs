namespace PsyLink.API.DTOs;

/// <summary>Weekly schedule block belonging to a psychologist.</summary>
public sealed record ScheduleResponseDto(
    Guid ScheduleId,
    Guid PsychologistId,
    string WeekDay,
    TimeOnly StartsAt,
    TimeOnly EndsAt);
