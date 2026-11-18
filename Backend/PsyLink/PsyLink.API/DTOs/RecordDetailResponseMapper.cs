using AppAppointmentResponseDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentResponseDto;
using AppPatientResponseDto = PsyLink.Application.Features.Patients.DTOs.PatientResponseDto;
using AppPrescriptionResponseDto = PsyLink.Application.Features.Prescriptions.DTOs.PrescriptionResponseDto;
using AppRecordDetailResponseDto = PsyLink.Application.Features.PatientRecords.DTOs.RecordDetailResponseDto;
using AppRecordDiagnosisResponseDto = PsyLink.Application.Features.Diagnoses.DTOs.RecordDiagnosisResponseDto;
using AppUserResponseDto = PsyLink.Application.Features.Users.DTOs.UserResponseDto;

namespace PsyLink.API.DTOs;

internal static class RecordDetailResponseMapper
{
    public static RecordDetailResponseDto ToResponse(AppRecordDetailResponseDto record) =>
        new(
            record.RecordId,
            ToResponse(record.Patient),
            record.Appointment is null ? null : ToResponse(record.Appointment),
            record.RiskScore,
            record.Diagnoses.Select(ToResponse).ToList(),
            record.Prescriptions.Select(ToResponse).ToList());

    public static PrescriptionResponseDto ToPrescriptionResponse(
        AppPrescriptionResponseDto prescription) =>
        ToResponse(prescription);

    private static PatientResponseDto ToResponse(AppPatientResponseDto patient) =>
        new(
            patient.PatientId,
            ToResponse(patient.User),
            patient.BirthDate,
            patient.Description,
            patient.Age);

    private static UserResponseDto ToResponse(AppUserResponseDto user) =>
        new(
            user.UserId,
            user.TenantId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone is null
                ? null
                : new PhoneNumberResponseDto(user.Phone.PhoneId, user.Phone.Number),
            new RoleResponseDto(user.Role.RoleId, user.Role.Name));

    private static AppointmentResponseDto ToResponse(AppAppointmentResponseDto appointment) =>
        new(
            appointment.AppointmentId,
            appointment.PatientId,
            appointment.PatientName,
            appointment.PsychologistId,
            appointment.PsychologistName,
            appointment.DateAt,
            appointment.StartsAt,
            appointment.EndsAt);

    private static RecordDiagnosisResponseDto ToResponse(
        AppRecordDiagnosisResponseDto diagnosis) =>
        new(
            diagnosis.RecordId,
            new DiagnosisDetailResponseDto(
                diagnosis.Diagnosis.DiagnosisId,
                diagnosis.Diagnosis.Name,
                diagnosis.Diagnosis.Description),
            diagnosis.Notes);

    private static PrescriptionResponseDto ToResponse(AppPrescriptionResponseDto prescription) =>
        new(
            prescription.PrescriptionId,
            prescription.RecordId,
            new MedicationResponseDto(
                prescription.Medication.MedicationId,
                prescription.Medication.Name,
                prescription.Medication.Content,
                prescription.Medication.Presentation,
                prescription.Medication.Laboratory is null
                    ? null
                    : new LaboratoryResponseDto(
                        prescription.Medication.Laboratory.LaboratoryId,
                        prescription.Medication.Laboratory.Name,
                        prescription.Medication.Laboratory.Phone is null
                            ? null
                            : new PhoneNumberResponseDto(
                                prescription.Medication.Laboratory.Phone.PhoneId,
                                prescription.Medication.Laboratory.Phone.Number)),
                prescription.Medication.Ingredients
                    .Select(ingredient => new IngredientResponseDto(
                        ingredient.IngredientId,
                        ingredient.Name))
                    .ToList()),
            prescription.Dosage,
            prescription.Quantity);
}
