using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;

namespace PsyLink.Application.Features.Psychologists.Services.Queries;

public record GetPsychologistProfileQuery(Guid PsychologistId)
    : IRequest<PsychologistDetailResponseDto>;
