using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;

namespace PsyLink.Application.Features.PatientRecords.DTOs;

public class RecordDetailResponseDto
{
    public Guid RecordId { get; set; }
    public PatientResponseDto Patient { get; set; } = new();
    public AppointmentResponseDto? Appointment { get; set; }
    public int RiskScore { get; set; }
    public List<AppointmentResponseDto> Appointments { get; set; } = new();
    public List<RecordDiagnosisResponseDto> Diagnoses { get; set; } = new();
    public List<PrescriptionResponseDto> Prescriptions { get; set; } = new();
}
