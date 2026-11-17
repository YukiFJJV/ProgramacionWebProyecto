using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Schedules.DTOs;

public class WorkScheduleResponseDto
{
    public Guid ScheduleId { get; set; }
    public Guid PsychologistId { get; set; }
    public WeekDay WeekDay { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
}
