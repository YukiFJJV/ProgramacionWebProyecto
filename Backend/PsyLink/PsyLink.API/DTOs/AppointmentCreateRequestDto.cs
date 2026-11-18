namespace PsyLink.API.DTOs;

/// <summary>Request to book an appointment for the authenticated patient or an eligible patient.</summary>
public sealed record AppointmentCreateRequestDto
{
    public Guid PsychologistId { get; init; }
    public Guid? PatientId { get; init; }
    public DateOnly DateAt { get; init; }
    public TimeOnly StartsAt { get; init; }
    public TimeOnly EndsAt { get; init; }
}
