using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Commands;
using PsyLink.Application.Features.Patients.Services.Handlers.GetPatientDiagnoses;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Patients.Services.Handlers.UpsertPatientDiagnosis;

public sealed class UpsertPatientDiagnosisHandler(
    IPatientRepository patientRepository,
    IPatientDiagnosisRepository patientDiagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpsertPatientDiagnosisCommand, PatientDiagnosisResponseDto>
{
    public async Task<PatientDiagnosisResponseDto> Handle(
        UpsertPatientDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        var scope = PatientAccess.EnsurePsychologist(currentUser);
        var patient = await patientRepository.GetByIdForPsychologistAsync(
            request.PatientId,
            scope.TenantId,
            scope.UserId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {request.PatientId}.");
        var diagnosis = await patientDiagnosisRepository.GetActiveDiagnosisByIdAsync(
            request.DiagnosisId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        var status = await patientDiagnosisRepository.GetStatusByIdAsync(
            request.StatusId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el estado {request.StatusId}.");
        var patientDiagnosis = await patientDiagnosisRepository.GetForUpdateAsync(
            patient.PatientId,
            diagnosis.DiagnosisId,
            scope.TenantId,
            cancellationToken);
        var isNew = patientDiagnosis is null;
        patientDiagnosis ??= new PatientDiagnosis
        {
            PatientId = patient.PatientId,
            DiagnosisId = diagnosis.DiagnosisId
        };
        patientDiagnosis.StatusId = status.StatusId;
        patientDiagnosis.UpdatedAt = DateTime.UtcNow;
        patientDiagnosis.Diagnosis = diagnosis;
        patientDiagnosis.Status = status;

        await patientDiagnosisRepository.SaveUpsertAsync(
            patientDiagnosis,
            isNew,
            cancellationToken);

        return GetPatientDiagnosesHandler.ToResponse(patientDiagnosis);
    }
}
