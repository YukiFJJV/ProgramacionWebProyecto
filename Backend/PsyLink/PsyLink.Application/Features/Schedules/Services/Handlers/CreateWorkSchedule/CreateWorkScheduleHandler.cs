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
    : IRequestHandler<CreateWorkScheduleCommand, WorkScheduleResponseDto>
{
    public async Task<WorkScheduleResponseDto> Handle(
        CreateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureCompanyAdmin();
        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId,
                cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");

        var overlapping = await workScheduleRepository.GetOverlappingAsync(
            psychologist.PsychologistId,
            tenantId,
            (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            request.Schedule.StartsAt,
            request.Schedule.EndsAt,
            cancellationToken: cancellationToken);
        if (overlapping.Count > 0)
        {
            throw new ValidationException("El horario se traslapa con otro bloque del psicólogo.");
        }

        var schedule = new WorkSchedule
        {
            ScheduleId = Guid.NewGuid(),
            PsychologistId = psychologist.PsychologistId,
            TenantId = tenantId,
            WeekDay = (PsyLink.Domain.Clinical.WeekDay)(int)request.Schedule.WeekDay,
            StartsAt = request.Schedule.StartsAt,
            EndsAt = request.Schedule.EndsAt,
            Psychologist = psychologist
        };
        if (!await workScheduleRepository.AddIfNoOverlapAsync(schedule, cancellationToken))
        {
            throw new ValidationException("El horario se traslapa con otro bloque del psicólogo.");
        }

        return ToResponse(schedule);
    }

    private Guid EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new UnauthorizedAccessException("La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }

    internal static WorkScheduleResponseDto ToResponse(WorkSchedule schedule) => new()
    {
        ScheduleId = schedule.ScheduleId,
        PsychologistId = schedule.PsychologistId,
        WeekDay = (PsyLink.Application.Features.Schedules.DTOs.WeekDay)(int)schedule.WeekDay,
        StartsAt = schedule.StartsAt,
        EndsAt = schedule.EndsAt
    };
}
