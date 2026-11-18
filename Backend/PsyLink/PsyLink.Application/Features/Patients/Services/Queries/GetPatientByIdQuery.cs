using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Queries;

public record GetPatientByIdQuery(Guid PatientId) : IQuery<PatientResponseDto>;
