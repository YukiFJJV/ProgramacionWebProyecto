using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record UpsertRecordDiagnosisCommand(
    Guid RecordId,
    Guid DiagnosisId,
    RecordDiagnosisRequestDto Diagnosis)
    : ICommand<RecordDiagnosisResponseDto>;
