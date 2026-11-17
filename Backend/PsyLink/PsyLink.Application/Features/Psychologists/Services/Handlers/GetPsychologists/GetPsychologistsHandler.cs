using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Psychologists.Services.Queries;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Psychologists.Services.Handlers.GetPsychologists;

public sealed class GetPsychologistsHandler(
    IPsychologistRepository psychologistRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPsychologistsQuery, PageResponseDto<PsychologistSummaryResponseDto>>
{
    public async Task<PageResponseDto<PsychologistSummaryResponseDto>> Handle(
        GetPsychologistsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if (currentUser.RoleName != RoleNames.Admin || tenantId is null)
        {
            throw new UnauthorizedAccessException("La operación requiere un administrador de empresa autenticado.");
        }

        var (psychologists, totalCount) = await psychologistRepository.SearchByTenantAsync(
            tenantId.Value,
            request.Search,
            request.SpecialtyId,
            request.Page,
            request.PageSize,
            cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PageResponseDto<PsychologistSummaryResponseDto>
        {
            Items = psychologists.Select(psychologist => new PsychologistSummaryResponseDto
            {
                PsychologistId = psychologist.PsychologistId,
                FirstName = psychologist.User.FirstName,
                LastName = psychologist.User.LastName,
                Specialty = new SpecialtyResponseDto
                {
                    SpecialtyId = psychologist.Specialty.SpecialtyId,
                    SpecialtyName = psychologist.Specialty.SpecialtyName
                },
                LicenseNumber = psychologist.LicenseNumber
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
            HasPrevious = request.Page > 1,
            HasNext = request.Page < totalPages
        };
    }
}
