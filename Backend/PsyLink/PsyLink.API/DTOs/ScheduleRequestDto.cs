using PsyLink.Application.Features.Schedules.DTOs;

namespace PsyLink.API.DTOs;

/// <summary>Payload for adding or updating a psychologist's weekly schedule block.</summary>
public sealed record ScheduleRequestDto(
    WeekDay WeekDay,
    TimeOnly StartsAt,
    TimeOnly EndsAt);
