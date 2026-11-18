using MediatR;
using PsyLink.Application.Features.Prescriptions.DTOs;

namespace PsyLink.Application.Features.Medications.Services.Queries;

public sealed record GetLaboratoriesQuery : IQuery<IReadOnlyList<LaboratoryResponseDto>>;
