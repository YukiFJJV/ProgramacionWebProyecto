using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;

namespace PsyLink.Application.Features.Schedules.Services.Queries;

public record GetWorkSchedulesQuery(Guid PsychologistId) : IRequest<IReadOnlyList<WorkScheduleResponseDto>>;
