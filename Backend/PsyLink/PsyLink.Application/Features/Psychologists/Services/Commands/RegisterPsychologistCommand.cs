using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;

namespace PsyLink.Application.Features.Psychologists.Services.Commands;

public record RegisterPsychologistCommand(PsychologistCreateRequestDto Psychologist)
    : IRequest<PsychologistDetailResponseDto>;
