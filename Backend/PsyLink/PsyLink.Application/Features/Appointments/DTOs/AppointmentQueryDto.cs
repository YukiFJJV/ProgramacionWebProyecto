namespace PsyLink.Application.Features.Appointments.DTOs;

public sealed record AppointmentQueryDto
{
    public string? Search { get; init; }
    public Guid? PatientId { get; init; }
    public Guid? PsychologistId { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortDirection { get; init; } = "asc";
}
