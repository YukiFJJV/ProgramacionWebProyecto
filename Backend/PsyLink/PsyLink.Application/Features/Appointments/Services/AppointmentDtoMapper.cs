using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Appointments.Services;

internal static class AppointmentDtoMapper
{
    public static AppointmentResponseDto ToResponse(Appointment appointment) => new()
    {
        AppointmentId = appointment.AppointmentId,
        PatientId = appointment.PatientId,
        PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
        PsychologistId = appointment.PsychologistId,
        PsychologistName = $"{appointment.Psychologist.User.FirstName} {appointment.Psychologist.User.LastName}",
        DateAt = appointment.DateAt,
        StartsAt = appointment.StartsAt,
        EndsAt = appointment.EndsAt
    };

    public static AppointmentDetailResponseDto ToDetail(Appointment appointment) => new()
    {
        AppointmentId = appointment.AppointmentId,
        Patient = ToPatient(appointment.Patient),
        Psychologist = new PsychologistSummaryResponseDto
        {
            PsychologistId = appointment.PsychologistId,
            FirstName = appointment.Psychologist.User.FirstName,
            LastName = appointment.Psychologist.User.LastName,
            Specialty = new SpecialtyResponseDto
            {
                SpecialtyId = appointment.Psychologist.Specialty.SpecialtyId,
                SpecialtyName = appointment.Psychologist.Specialty.SpecialtyName
            },
            LicenseNumber = appointment.Psychologist.LicenseNumber
        },
        DateAt = appointment.DateAt,
        StartsAt = appointment.StartsAt,
        EndsAt = appointment.EndsAt,
        Status = appointment.Status
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
