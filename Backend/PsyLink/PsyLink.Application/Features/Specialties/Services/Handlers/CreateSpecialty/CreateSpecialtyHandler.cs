using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Specialties.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Specialties.Services.Handlers.CreateSpecialty;

public sealed class CreateSpecialtyHandler(
    ISpecialtyRepository specialtyRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateSpecialtyCommand, SpecialtyResponseDto>
{
    public async Task<SpecialtyResponseDto> Handle(
        CreateSpecialtyCommand request,
        CancellationToken cancellationToken)
    {
        EnsureAuthorized();

        if (await specialtyRepository.ExistsByNameAsync(
                request.SpecialtyName,
                cancellationToken: cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException(
                "Ya existe una especialidad con ese nombre.");
        }

        var specialty = new Specialty
        {
            SpecialtyId = Guid.NewGuid(),
            SpecialtyName = request.SpecialtyName
        };
        await specialtyRepository.AddAsync(specialty, cancellationToken);

        return new SpecialtyResponseDto
        {
            SpecialtyId = specialty.SpecialtyId,
            SpecialtyName = specialty.SpecialtyName
        };
    }

    private void EnsureAuthorized()
    {
        if (currentUser.RoleName != RoleNames.Admin &&
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere el rol ADMIN o PSYCHOLOGIST.");
        }
    }
}
