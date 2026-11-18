using MediatR;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Prescriptions.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Prescriptions.Services.Handlers.UpdatePrescription;

public sealed class UpdatePrescriptionHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    IPrescriptionRepository prescriptionRepository,
    IMedicationRepository medicationRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdatePrescriptionCommand, PrescriptionResponseDto>
{
    public async Task<PrescriptionResponseDto> Handle(
        UpdatePrescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var (_, tenantId) = await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);
        var prescription = await prescriptionRepository.GetByIdForRecordAsync(
                request.PrescriptionId,
                request.RecordId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la receta {request.PrescriptionId}.");

        if (request.Update.MedicationId is Guid medicationId)
        {
            var medication = await medicationRepository.GetByIdAsync(medicationId, cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el medicamento {medicationId}.");
            prescription.ChangeMedication(medication.MedicationId);
        }

        if (request.Update.Dosage is not null)
        {
            prescription.Update(
                request.Update.Dosage.Trim(),
                prescription.Quantity ?? 0);
        }

        if (request.Update.Quantity is int quantity)
        {
            prescription.Update(prescription.Dosage, quantity);
        }

        await prescriptionRepository.UpdateAsync(prescription, cancellationToken);
        return PatientRecordDtoMapper.ToPrescription(prescription);
    }
}
