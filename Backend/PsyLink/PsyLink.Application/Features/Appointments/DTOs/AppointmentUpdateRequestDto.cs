namespace PsyLink.Application.Features.Appointments.DTOs;

public sealed record AppointmentUpdateRequestDto
{
    public Guid? PsychologistId { get; init; }
    public DateOnly? DateAt { get; init; }
    public TimeOnly? StartsAt { get; init; }
    public TimeOnly? EndsAt { get; init; }
}
