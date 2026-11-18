namespace PsyLink.API.DTOs;

/// <summary>Fields that can be changed when rescheduling an appointment.</summary>
public sealed record AppointmentUpdateRequestDto
{
    public DateOnly? DateAt { get; init; }
    public TimeOnly? StartsAt { get; init; }
    public TimeOnly? EndsAt { get; init; }
}
