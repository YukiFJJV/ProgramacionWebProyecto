using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Queries;

public sealed record GetPatientDiagnosesQuery(Guid PatientId)
    : IQuery<IReadOnlyList<PatientDiagnosisResponseDto>>;
