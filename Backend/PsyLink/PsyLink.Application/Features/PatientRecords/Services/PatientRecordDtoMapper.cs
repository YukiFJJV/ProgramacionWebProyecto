using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Features.PatientRecords.Services;

internal static class PatientRecordDtoMapper
{
    public static RecordDetailResponseDto ToDetail(
        PatientRecord record,
        IReadOnlyList<Appointment> appointments,
        int? effectiveRiskScore = null) => new()
    {
        RecordId = record.RecordId,
        Patient = ToPatient(record.Patient),
        Appointment = record.Appointment is null
            ? null
            : AppointmentDtoMapper.ToResponse(record.Appointment),
        RiskScore = effectiveRiskScore ?? record.RiskScore,
        Appointments = appointments.Select(AppointmentDtoMapper.ToResponse).ToList(),
        Diagnoses = record.Diagnoses.Select(ToRecordDiagnosis).ToList(),
        Prescriptions = record.Prescriptions.Select(ToPrescription).ToList()
    };

    public static PrescriptionResponseDto ToPrescription(Prescription prescription) => new()
    {
        PrescriptionId = prescription.PrescriptionId,
        RecordId = prescription.RecordId,
        Medication = new MedicationResponseDto
        {
            MedicationId = prescription.Medication.MedicationId,
            Name = prescription.Medication.Name,
            Content = prescription.Medication.Content,
            Presentation = prescription.Medication.Presentation,
            Laboratory = prescription.Medication.Laboratory is null
                ? null
                : new LaboratoryResponseDto
                {
                    LaboratoryId = prescription.Medication.Laboratory.LaboratoryId,
                    Name = prescription.Medication.Laboratory.Name,
                    Phone = prescription.Medication.Laboratory.Phone is null
                        ? null
                        : new PhoneNumberResponseDto
                        {
                            PhoneId = prescription.Medication.Laboratory.Phone.PhoneId,
                            Number = prescription.Medication.Laboratory.Phone.Number
                        }
                },
            Ingredients = prescription.Medication.Ingredients
                .Select(medicationIngredient => new IngredientResponseDto
                {
                    IngredientId = medicationIngredient.Ingredient.IngredientId,
                    Name = medicationIngredient.Ingredient.Name
                })
                .ToList()
        },
        Dosage = prescription.Dosage,
        Quantity = prescription.Quantity
    };

    public static RecordDiagnosisResponseDto ToRecordDiagnosis(RecordDiagnosis recordDiagnosis) => new()
    {
        RecordId = recordDiagnosis.RecordId,
        Diagnosis = new DiagnosisDetailResponseDto
        {
            DiagnosisId = recordDiagnosis.Diagnosis.DiagnosisId,
            Name = recordDiagnosis.Diagnosis.Name,
            Description = recordDiagnosis.Diagnosis.Description
        },
        Notes = recordDiagnosis.Notes
    };

    private static PatientResponseDto ToPatient(Patient patient) => new()
    {
        PatientId = patient.PatientId,
        User = new UserResponseDto
        {
            UserId = patient.UserId,
            TenantId = patient.TenantId,
            Email = patient.User.Email,
            FirstName = patient.User.FirstName,
            LastName = patient.User.LastName,
            Phone = patient.User.Phone is null
                ? null
                : new PhoneNumberResponseDto
                {
                    PhoneId = patient.User.Phone.PhoneId,
                    Number = patient.User.Phone.Number
                },
            Role = new RoleResponseDto
            {
                RoleId = patient.User.Role.RoleId,
                Name = patient.User.Role.Name
            }
        },
        BirthDate = patient.BirthDate,
        Description = patient.Description,
        Age = patient.AgeOn(DateOnly.FromDateTime(DateTime.UtcNow))
    };
}
