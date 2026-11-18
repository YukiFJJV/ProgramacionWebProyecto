namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentQueryDto
{
    public string? Search { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? PsychologistId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortDirection { get; set; } = "asc";
}
