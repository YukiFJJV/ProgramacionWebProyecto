using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.Application.Features.Schedules.Services.Handlers.CreateWorkSchedule;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Schedules.Services.Handlers.UpdateWorkSchedule;

public sealed class UpdateWorkScheduleHandler(
    IWorkScheduleRepository workScheduleRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateWorkScheduleCommand, WorkScheduleResponseDto>
{
    public async Task<WorkScheduleResponseDto> Handle(
        UpdateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureCompanyAdmin();
        var schedule = await workScheduleRepository.GetByIdForTenantAsync(
                request.ScheduleId,
                tenantId,
                cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el horario {request.ScheduleId}.");

        var overlapping = await workScheduleRepository.GetOverlappingAsync(
            schedule.PsychologistId,
            tenantId,
            request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt,
            schedule.ScheduleId,
            cancellationToken);
        if (overlapping.Count > 0)
        {
            throw new ValidationException("El horario se traslapa con otro bloque del psicólogo.");
        }

        schedule.WeekDay = request.Schedule.WeekDay;
        schedule.StartsAt = request.Schedule.StartsAt;
        schedule.EndsAt = request.Schedule.EndsAt;

        if (!await workScheduleRepository.UpdateIfNoOverlapAsync(schedule, cancellationToken))
        {
            throw new ValidationException("El horario se traslapa con otro bloque del psicólogo.");
        }

        return CreateWorkScheduleHandler.ToResponse(schedule);
    }

    private Guid EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new UnauthorizedAccessException("La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }
}
