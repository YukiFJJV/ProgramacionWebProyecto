namespace PsyLink.API.DTOs;

/// <summary>Filters and pagination options for tenant appointment searches.</summary>
public sealed record AppointmentQueryDto
{
    public Guid? PatientId { get; init; }
    public Guid? PsychologistId { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortDirection { get; init; } = "asc";
}
