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
    : IRequestHandler<UpdateWorkScheduleCommand, ScheduleResponseDto>
{
    public async Task<ScheduleResponseDto> Handle(
        UpdateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureCompanyAdmin();
        var schedule = await workScheduleRepository.GetByIdForTenantAsync(
                request.ScheduleId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el horario {request.ScheduleId}.");

        var overlapping = await workScheduleRepository.GetOverlappingAsync(
            schedule.PsychologistId,
            tenantId,
            (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt,
            schedule.ScheduleId,
            cancellationToken);
        if (overlapping.Count > 0)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario se traslapa con otro bloque del psicólogo.");
        }

        schedule.Update(
            (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt);

        if (!await workScheduleRepository.UpdateIfNoOverlapAsync(schedule, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario se traslapa con otro bloque del psicólogo.");
        }

        return CreateWorkScheduleHandler.ToResponse(schedule);
    }

    private Guid EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }
}
