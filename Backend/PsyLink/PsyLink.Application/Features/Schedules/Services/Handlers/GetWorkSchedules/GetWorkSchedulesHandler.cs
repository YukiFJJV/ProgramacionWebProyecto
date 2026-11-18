using MediatR;
using PsyLink.Application.Features.Schedules.DTOs;
using PsyLink.Application.Features.Schedules.Services.Handlers.CreateWorkSchedule;
using PsyLink.Application.Features.Schedules.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Schedules.Services.Handlers.GetWorkSchedules;

public sealed class GetWorkSchedulesHandler(
    IPsychologistRepository psychologistRepository,
    IWorkScheduleRepository workScheduleRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetWorkSchedulesQuery, IReadOnlyList<WorkScheduleResponseDto>>
{
    public async Task<IReadOnlyList<WorkScheduleResponseDto>> Handle(
        GetWorkSchedulesQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if (tenantId is null ||
            (currentUser.RoleName != RoleNames.Admin && currentUser.RoleName != RoleNames.Patient))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN o PATIENT.");
        }

        _ = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            request.PsychologistId,
            tenantId.Value,
            cancellationToken);
        return schedules.Select(CreateWorkScheduleHandler.ToResponse).ToList();
    }
}
