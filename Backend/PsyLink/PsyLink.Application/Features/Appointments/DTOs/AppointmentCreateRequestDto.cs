namespace PsyLink.Application.Features.Appointments.DTOs;

public sealed record AppointmentCreateRequestDto
{
    public Guid PsychologistId { get; init; }
    public Guid? PatientId { get; init; }
    public DateOnly DateAt { get; init; }
    public TimeOnly StartsAt { get; init; }
    public TimeOnly EndsAt { get; init; }
}
