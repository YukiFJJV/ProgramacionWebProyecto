using MediatR;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Prescriptions.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Features.Prescriptions.Services.Handlers.CreatePrescription;

public sealed class CreatePrescriptionHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    IPrescriptionRepository prescriptionRepository,
    IMedicationRepository medicationRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePrescriptionCommand, PrescriptionResponseDto>
{
    public async Task<PrescriptionResponseDto> Handle(
        CreatePrescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var (record, _) = await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);
        var medication = await medicationRepository.GetByIdAsync(
                request.Prescription.MedicationId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException(
                $"No se encontró el medicamento {request.Prescription.MedicationId}.");

        var prescription = new Prescription
        {
            PrescriptionId = Guid.NewGuid(),
            RecordId = record.RecordId,
            MedicationId = medication.MedicationId,
            Dosage = request.Prescription.Dosage.Trim(),
            Quantity = request.Prescription.Quantity,
            Record = record,
            Medication = medication
        };
        await prescriptionRepository.AddAsync(prescription, cancellationToken);
        return PatientRecordDtoMapper.ToPrescription(prescription);
    }
}
