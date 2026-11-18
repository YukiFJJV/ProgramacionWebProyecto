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
    IPatientRepository patientRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPsychologistAppointmentsQuery, PageResponseDto<AppointmentResponseDto>>
{
    public async Task<PageResponseDto<AppointmentResponseDto>> Handle(
        GetPsychologistAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not Guid tenantId)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("Se requiere una cuenta autenticada con acceso a un tenant.");
        }

        var filter = request.Filter;
        var patientId = filter.PatientId;
        var psychologistId = filter.PsychologistId;

        if (currentUser.RoleName == RoleNames.Psychologist)
        {
            var psychologist = await psychologistRepository.GetByUserIdForTenantAsync(
                    currentUser.UserId,
                    tenantId,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de psicólogo autenticado.");
            psychologistId = psychologist.PsychologistId;
        }
        else if (currentUser.RoleName == RoleNames.Patient)
        {
            var patient = await patientRepository.GetByUserIdAsync(
                    currentUser.UserId,
                    tenantId,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de paciente autenticado.");
            patientId = patient.PatientId;
        }
        else if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador, paciente o psicólogo.");
        }

        var result = await appointmentRepository.GetByTenantWithPatientSearchAsync(
            tenantId,
            filter.Search,
            patientId,
            psychologistId,
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
