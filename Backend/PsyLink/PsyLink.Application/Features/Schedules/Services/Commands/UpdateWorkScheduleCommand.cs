using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;

namespace PsyLink.Application.Features.Schedules.Services.Commands;

public record UpdateWorkScheduleCommand(Guid ScheduleId, ScheduleRequestDto Schedule)
    : IRequest<WorkScheduleResponseDto>;
