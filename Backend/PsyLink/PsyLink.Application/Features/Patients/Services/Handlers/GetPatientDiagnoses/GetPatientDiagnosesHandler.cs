using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Queries;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Patients.Services.Handlers.GetPatientDiagnoses;

public sealed class GetPatientDiagnosesHandler(
    IPatientRepository patientRepository,
    IPatientDiagnosisRepository patientDiagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPatientDiagnosesQuery, IReadOnlyList<PatientDiagnosisResponseDto>>
{
    public async Task<IReadOnlyList<PatientDiagnosisResponseDto>> Handle(
        GetPatientDiagnosesQuery request,
        CancellationToken cancellationToken)
    {
        var scope = PatientAccess.EnsurePsychologist(currentUser);
        var patient = await patientRepository.GetByIdForPsychologistAsync(
            request.PatientId,
            scope.TenantId,
            scope.UserId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {request.PatientId}.");
        var diagnoses = await patientDiagnosisRepository.GetByPatientIdAsync(
            patient.PatientId,
            scope.TenantId,
            cancellationToken);

        return diagnoses.Select(ToResponse).ToList();
    }

    internal static PatientDiagnosisResponseDto ToResponse(
        PsyLink.Domain.Clinical.PatientDiagnosis patientDiagnosis) =>
        new()
        {
            PatientId = patientDiagnosis.PatientId,
            Diagnosis = new DiagnosisDetailResponseDto
            {
                DiagnosisId = patientDiagnosis.Diagnosis.DiagnosisId,
                Name = patientDiagnosis.Diagnosis.Name,
                Description = patientDiagnosis.Diagnosis.Description
            },
            Status = new DiagnosisStatusResponseDto
            {
                StatusId = patientDiagnosis.Status.StatusId,
                Name = patientDiagnosis.Status.Name
            },
            UpdatedAt = patientDiagnosis.UpdatedAt
        };
}
