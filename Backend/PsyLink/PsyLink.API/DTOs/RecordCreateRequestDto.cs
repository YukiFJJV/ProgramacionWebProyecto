namespace PsyLink.API.DTOs;

/// <summary>Initial patient record data; tenant and psychologist are derived from the session.</summary>
public sealed record RecordCreateRequestDto
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public int RiskScore { get; init; }
}
