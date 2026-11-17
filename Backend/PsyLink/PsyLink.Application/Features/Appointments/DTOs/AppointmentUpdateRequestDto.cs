namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentUpdateRequestDto
{
    public Guid? PsychologistId { get; set; }
    public DateOnly? DateAt { get; set; }
    public TimeOnly? StartsAt { get; set; }
    public TimeOnly? EndsAt { get; set; }
}
