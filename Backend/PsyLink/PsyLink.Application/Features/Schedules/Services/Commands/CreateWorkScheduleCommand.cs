using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;

namespace PsyLink.Application.Features.Schedules.Services.Commands;

public record CreateWorkScheduleCommand(Guid PsychologistId, ScheduleRequestDto Schedule)
    : ICommand<ScheduleResponseDto>;
