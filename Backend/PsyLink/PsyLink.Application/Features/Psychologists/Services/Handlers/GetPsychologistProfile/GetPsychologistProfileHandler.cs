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
        if (currentUser.RoleName != RoleNames.Patient || tenantId is null)
        {
            throw new UnauthorizedAccessException("La consulta del perfil requiere un paciente autenticado.");
        }

        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");

        if (!psychologist.User.Active)
        {
            throw new KeyNotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");
        }

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            psychologist.PsychologistId,
            tenantId.Value,
            cancellationToken);

        return new PsychologistDetailResponseDto
        {
            PsychologistId = psychologist.PsychologistId,
            User = CreateUserHandler.ToResponse(psychologist.User),
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
