using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record UpdateDiagnosisCommand(Guid DiagnosisId, DiagnosisRequestDto Diagnosis)
    : ICommand<DiagnosisDetailResponseDto>;
