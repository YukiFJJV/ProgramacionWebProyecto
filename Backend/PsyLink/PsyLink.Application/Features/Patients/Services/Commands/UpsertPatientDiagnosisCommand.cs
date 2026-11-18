using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Patients.Services.Commands;

public sealed record UpsertPatientDiagnosisCommand(
    Guid PatientId,
    Guid DiagnosisId,
    Guid StatusId,
    string? Notes) : ICommand<PatientDiagnosisResponseDto>;
