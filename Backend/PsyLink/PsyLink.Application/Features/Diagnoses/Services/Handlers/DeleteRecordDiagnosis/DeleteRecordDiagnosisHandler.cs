using MediatR;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.DeleteRecordDiagnosis;

public sealed class DeleteRecordDiagnosisHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    IRecordDiagnosisRepository recordDiagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteRecordDiagnosisCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteRecordDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);
        var recordDiagnosis = await recordDiagnosisRepository.GetByRecordAndDiagnosisAsync(
                request.RecordId,
                request.DiagnosisId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException(
                $"No se encontró el diagnóstico {request.DiagnosisId} en el expediente {request.RecordId}.");

        await recordDiagnosisRepository.DeleteAsync(recordDiagnosis, cancellationToken);
        return Unit.Value;
    }
}
