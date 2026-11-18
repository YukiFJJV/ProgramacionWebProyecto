using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Patients.Services.Queries;

public sealed record GetDiagnosisStatusesQuery
    : IQuery<IReadOnlyList<DiagnosisStatusResponseDto>>;
