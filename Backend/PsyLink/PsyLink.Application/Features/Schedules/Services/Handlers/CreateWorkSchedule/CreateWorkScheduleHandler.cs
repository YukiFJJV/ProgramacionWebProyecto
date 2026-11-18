using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Schedules.Services.Handlers.CreateWorkSchedule;

public sealed class CreateWorkScheduleHandler(
    IPsychologistRepository psychologistRepository,
    IWorkScheduleRepository workScheduleRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateWorkScheduleCommand, ScheduleResponseDto>
{
    public async Task<ScheduleResponseDto> Handle(
        CreateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureCompanyAdmin();
        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");

        var overlapping = await workScheduleRepository.GetOverlappingAsync(
            psychologist.PsychologistId,
            tenantId,
            (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt,
            cancellationToken: cancellationToken);
        if (overlapping.Count > 0)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario se traslapa con otro bloque del psicólogo.");
        }

        var schedule = WorkSchedule.Create(
            psychologist.PsychologistId,
            tenantId,
            (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt);
        if (!await workScheduleRepository.AddIfNoOverlapAsync(schedule, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario se traslapa con otro bloque del psicólogo.");
        }

        return ToResponse(schedule);
    }

    private Guid EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }

    internal static ScheduleResponseDto ToResponse(WorkSchedule schedule) =>
        new(
            schedule.ScheduleId,
            schedule.PsychologistId,
            schedule.WeekDay.ToString().ToUpperInvariant(),
            schedule.StartsAt,
            schedule.EndsAt);
}
