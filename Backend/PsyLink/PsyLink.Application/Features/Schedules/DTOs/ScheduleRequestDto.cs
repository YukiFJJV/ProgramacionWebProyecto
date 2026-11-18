namespace PsyLink.Application.Features.Schedules.DTOs;

public enum WeekDay
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public class ScheduleRequestDto
{
    public WeekDay WeekDay { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
}
