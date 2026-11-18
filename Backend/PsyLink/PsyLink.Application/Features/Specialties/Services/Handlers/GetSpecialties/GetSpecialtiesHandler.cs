using MediatR;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Specialties.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Specialties.Services.Handlers.GetSpecialties;

public sealed class GetSpecialtiesHandler(
    ISpecialtyRepository specialtyRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetSpecialtiesQuery, IReadOnlyList<SpecialtyResponseDto>>
{
    public async Task<IReadOnlyList<SpecialtyResponseDto>> Handle(
        GetSpecialtiesQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin &&
            currentUser.RoleName != RoleNames.Psychologist &&
            currentUser.RoleName != RoleNames.Patient)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere el rol ADMIN, PSYCHOLOGIST o PATIENT.");
        }

        var specialties = await specialtyRepository.GetAllAsync(cancellationToken);
        return specialties.Select(specialty => new SpecialtyResponseDto
        {
            SpecialtyId = specialty.SpecialtyId,
            SpecialtyName = specialty.SpecialtyName
        }).ToList();
    }
}
