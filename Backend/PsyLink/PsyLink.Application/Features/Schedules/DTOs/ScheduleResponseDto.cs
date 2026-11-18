namespace PsyLink.Application.Features.Schedules.DTOs;

public sealed record ScheduleResponseDto(
    Guid ScheduleId,
    Guid PsychologistId,
    string WeekDay,
    TimeOnly StartsAt,
    TimeOnly EndsAt);
