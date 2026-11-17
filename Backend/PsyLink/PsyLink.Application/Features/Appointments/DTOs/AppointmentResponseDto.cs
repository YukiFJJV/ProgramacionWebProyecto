namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentResponseDto
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid PsychologistId { get; set; }
    public string? PsychologistName { get; set; }
    public DateOnly DateAt { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
}
