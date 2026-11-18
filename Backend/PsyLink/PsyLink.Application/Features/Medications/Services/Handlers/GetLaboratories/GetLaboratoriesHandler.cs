using MediatR;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Medications.Services.Handlers.GetLaboratories;

public sealed class GetLaboratoriesHandler(
    ILaboratoryRepository laboratoryRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetLaboratoriesQuery, IReadOnlyList<LaboratoryResponseDto>>
{
    public async Task<IReadOnlyList<LaboratoryResponseDto>> Handle(
        GetLaboratoriesQuery request,
        CancellationToken cancellationToken)
    {
        EnsureAdmin();
        var laboratories = await laboratoryRepository.GetAllAsync(cancellationToken);
        return laboratories.Select(laboratory => new LaboratoryResponseDto
        {
            LaboratoryId = laboratory.LaboratoryId,
            Name = laboratory.Name,
            Phone = laboratory.Phone is null
                ? null
                : new PhoneNumberResponseDto
                {
                    PhoneId = laboratory.Phone.PhoneId,
                    PhoneNumber = laboratory.Phone.Number
                }
        }).ToList();
    }

    private void EnsureAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }
    }
}
