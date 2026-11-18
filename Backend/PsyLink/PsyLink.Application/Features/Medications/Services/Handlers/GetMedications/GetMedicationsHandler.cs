using MediatR;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Medications.Services.Handlers.GetMedications;

public sealed class GetMedicationsHandler(
    IMedicationRepository medicationRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMedicationsQuery, PageResponseDto<MedicationResponseDto>>
{
    public async Task<PageResponseDto<MedicationResponseDto>> Handle(
        GetMedicationsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin &&
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La consulta del catálogo requiere el rol ADMIN o PSYCHOLOGIST.");
        }

        var filter = request.Filter;
        var (medications, totalCount) = await medicationRepository.SearchAsync(
            filter.Search,
            filter.LaboratoryId,
            filter.IngredientId,
            filter.Page,
            filter.PageSize,
            cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        return new PageResponseDto<MedicationResponseDto>(
            medications.Select(MedicationDtoMapper.ToResponse).ToList(),
            filter.Page,
            filter.PageSize,
            totalCount,
            totalPages,
            filter.Page > 1,
            filter.Page < totalPages);
    }
}
