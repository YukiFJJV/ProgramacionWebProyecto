using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record CreateDiagnosisCommand(DiagnosisRequestDto Diagnosis)
    : ICommand<DiagnosisDetailResponseDto>;
