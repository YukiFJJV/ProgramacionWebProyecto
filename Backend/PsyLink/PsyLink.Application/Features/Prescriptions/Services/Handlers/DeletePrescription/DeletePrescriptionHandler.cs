using MediatR;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.Prescriptions.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Prescriptions.Services.Handlers.DeletePrescription;

public sealed class DeletePrescriptionHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    IPrescriptionRepository prescriptionRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<DeletePrescriptionCommand, Unit>
{
    public async Task<Unit> Handle(
        DeletePrescriptionCommand request,
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
            ?? throw new KeyNotFoundException($"No se encontró la receta {request.PrescriptionId}.");
        await prescriptionRepository.DeleteAsync(prescription, cancellationToken);
        return Unit.Value;
    }
}
