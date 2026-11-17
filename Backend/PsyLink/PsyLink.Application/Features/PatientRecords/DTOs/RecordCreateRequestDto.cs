namespace PsyLink.Application.Features.PatientRecords.DTOs;

public class RecordCreateRequestDto
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public int RiskScore { get; set; }
}
