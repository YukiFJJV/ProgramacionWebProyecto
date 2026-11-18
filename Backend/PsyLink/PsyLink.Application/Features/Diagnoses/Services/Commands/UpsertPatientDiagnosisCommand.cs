using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record UpsertPatientDiagnosisCommand(
    Guid PatientId,
    Guid DiagnosisId,
    Guid StatusId,
    string? Notes) : ICommand<PatientDiagnosisResponseDto>;
