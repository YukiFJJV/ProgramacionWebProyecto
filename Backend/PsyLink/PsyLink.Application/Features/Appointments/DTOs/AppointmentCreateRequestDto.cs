namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentCreateRequestDto
{
    public Guid PsychologistId { get; set; }
    public Guid? PatientId { get; set; }
    public DateOnly DateAt { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
}
