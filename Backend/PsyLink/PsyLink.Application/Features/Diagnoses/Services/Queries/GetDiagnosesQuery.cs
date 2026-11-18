using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Queries;

public sealed record GetDiagnosesQuery(DiagnosisQueryDto Filter)
    : IQuery<PageResponseDto<DiagnosisResponseDto>>;
