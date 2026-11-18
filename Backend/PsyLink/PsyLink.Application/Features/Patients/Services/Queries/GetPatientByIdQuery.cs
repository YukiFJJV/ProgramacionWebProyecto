using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Patients.Services.Queries;

public record GetPatientByIdQuery(Guid PatientId) : IQuery<PatientResponseDto>;
