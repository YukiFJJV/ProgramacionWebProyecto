using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Schedules.DTOs;

public class ScheduleRequestDto
{
    public WeekDay WeekDay { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
}
