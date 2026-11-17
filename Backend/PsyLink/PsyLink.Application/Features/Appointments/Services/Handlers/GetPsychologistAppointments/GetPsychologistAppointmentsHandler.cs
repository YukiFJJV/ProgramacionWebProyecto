using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Queries;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.GetPsychologistAppointments;

public sealed class GetPsychologistAppointmentsHandler(
    IAppointmentRepository appointmentRepository,
    IPsychologistRepository psychologistRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPsychologistAppointmentsQuery, PageResponseDto<AppointmentResponseDto>>
{
    public async Task<PageResponseDto<AppointmentResponseDto>> Handle(
        GetPsychologistAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not Guid tenantId ||
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new UnauthorizedAccessException("Solo un psicólogo autenticado puede consultar estas citas.");
        }

        var psychologist = await psychologistRepository.GetByUserIdForTenantAsync(
                currentUser.UserId,
                tenantId,
                cancellationToken)
            ?? throw new KeyNotFoundException("No se encontró el perfil de psicólogo autenticado.");

        var filter = request.Filter;
        var result = await appointmentRepository.GetByPsychologistWithPatientSearchAsync(
            psychologist.PsychologistId,
            tenantId,
            filter.Search,
            filter.PatientId,
            filter.DateFrom,
            filter.DateTo,
            filter.Page,
            filter.PageSize,
            filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase),
            cancellationToken);

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)filter.PageSize);
        return new PageResponseDto<AppointmentResponseDto>
        {
            Items = result.Items.Select(AppointmentDtoMapper.ToResponse).ToList(),
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalItems = result.TotalCount,
            TotalPages = totalPages,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages
        };
    }
}
