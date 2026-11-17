using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Specialties.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Specialties.Services.Handlers.UpdateSpecialty;

public sealed class UpdateSpecialtyHandler(
    ISpecialtyRepository specialtyRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateSpecialtyCommand, SpecialtyResponseDto>
{
    public async Task<SpecialtyResponseDto> Handle(
        UpdateSpecialtyCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin &&
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new UnauthorizedAccessException(
                "La operación requiere el rol ADMIN o PSYCHOLOGIST.");
        }

        var specialty = await specialtyRepository.GetByIdAsync(request.SpecialtyId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la especialidad {request.SpecialtyId}.");

        if (await specialtyRepository.ExistsByNameAsync(
                request.SpecialtyName,
                request.SpecialtyId,
                cancellationToken))
        {
            throw new ValidationException(
                new[]
                {
                    new ValidationFailure(
                        nameof(request.SpecialtyName),
                        "Ya existe una especialidad con ese nombre.")
                });
        }

        specialty.SpecialtyName = request.SpecialtyName;
        await specialtyRepository.UpdateAsync(specialty, cancellationToken);

        return new SpecialtyResponseDto
        {
            SpecialtyId = specialty.SpecialtyId,
            SpecialtyName = specialty.SpecialtyName
        };
    }
}
