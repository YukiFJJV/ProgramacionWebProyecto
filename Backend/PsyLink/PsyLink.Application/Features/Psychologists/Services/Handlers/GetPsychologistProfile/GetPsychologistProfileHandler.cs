using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Psychologists.Services.Queries;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Schedules.Services.Handlers.CreateWorkSchedule;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Psychologists.Services.Handlers.GetPsychologistProfile;

public sealed class GetPsychologistProfileHandler(
    IPsychologistRepository psychologistRepository,
    IWorkScheduleRepository workScheduleRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPsychologistProfileQuery, PsychologistDetailResponseDto>
{
    public async Task<PsychologistDetailResponseDto> Handle(
        GetPsychologistProfileQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if ((currentUser.RoleName != RoleNames.Admin &&
             currentUser.RoleName != RoleNames.Patient &&
             currentUser.RoleName != RoleNames.Psychologist) ||
            tenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La consulta del perfil requiere un administrador, paciente o psicólogo autenticado.");
        }

        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");

        if (!psychologist.User.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");
        }

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            psychologist.PsychologistId,
            tenantId.Value,
            cancellationToken);

        return new PsychologistDetailResponseDto
        {
            PsychologistId = psychologist.PsychologistId,
            User = PsyLink.Application.Features.Users.Shared.UserDtoMapper.ToResponse(psychologist.User),
            Specialty = new SpecialtyResponseDto
            {
                SpecialtyId = psychologist.Specialty.SpecialtyId,
                SpecialtyName = psychologist.Specialty.SpecialtyName
            },
            LicenseNumber = psychologist.LicenseNumber,
            Biography = psychologist.Biography,
            Schedules = schedules.Select(CreateWorkScheduleHandler.ToResponse).ToList()
        };
    }
}
