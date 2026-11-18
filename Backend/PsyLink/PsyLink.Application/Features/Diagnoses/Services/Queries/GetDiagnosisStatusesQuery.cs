using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Queries;

public sealed record GetDiagnosisStatusesQuery
    : IQuery<IReadOnlyList<DiagnosisStatusResponseDto>>;
