using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.UpsertRecordDiagnosis;

public sealed class UpsertRecordDiagnosisHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    IRecordDiagnosisRepository recordDiagnosisRepository,
    IDiagnosisCatalogRepository diagnosisCatalogRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpsertRecordDiagnosisCommand, RecordDiagnosisResponseDto>
{
    public async Task<RecordDiagnosisResponseDto> Handle(
        UpsertRecordDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        var (record, _) = await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);
        var diagnosis = await diagnosisCatalogRepository.GetByIdAsync(
                request.DiagnosisId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");

        if (!diagnosis.IsActive)
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException("No se puede asociar un diagnóstico inactivo al expediente.");
        }

        var recordDiagnosis = await recordDiagnosisRepository.GetByRecordAndDiagnosisAsync(
            record.RecordId,
            diagnosis.DiagnosisId,
            cancellationToken);
        var isNew = recordDiagnosis is null;
        if (recordDiagnosis is null)
        {
            recordDiagnosis = new RecordDiagnosis
            {
                RecordId = record.RecordId,
                DiagnosisId = diagnosis.DiagnosisId,
                Record = record,
                Diagnosis = diagnosis
            };
        }
        else
        {
            recordDiagnosis.Diagnosis = diagnosis;
        }

        recordDiagnosis.Notes = request.Diagnosis.Notes?.Trim();
        if (recordDiagnosis.Notes is not null && recordDiagnosis.Notes.Length == 0)
        {
            recordDiagnosis.Notes = null;
        }

        if (isNew)
        {
            await recordDiagnosisRepository.AddAsync(recordDiagnosis, cancellationToken);
        }
        else
        {
            await recordDiagnosisRepository.UpdateAsync(recordDiagnosis, cancellationToken);
        }

        return new RecordDiagnosisResponseDto
        {
            RecordId = recordDiagnosis.RecordId,
            Diagnosis = new DiagnosisDetailResponseDto
            {
                DiagnosisId = diagnosis.DiagnosisId,
                Name = diagnosis.Name,
                Description = diagnosis.Description
            },
            Notes = recordDiagnosis.Notes
        };
    }
}
