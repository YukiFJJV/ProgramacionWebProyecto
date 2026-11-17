using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;

namespace PsyLink.Application.Features.Psychologists.Services.Queries;

public record GetPsychologistAvailabilityQuery(
    Guid PsychologistId,
    AvailabilityQueryDto Availability)
    : IRequest<IReadOnlyList<AvailabilityResponseDto>>;
