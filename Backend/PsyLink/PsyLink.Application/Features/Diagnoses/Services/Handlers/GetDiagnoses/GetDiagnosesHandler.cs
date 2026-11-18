using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.GetDiagnoses;

public sealed class GetDiagnosesHandler(
    IDiagnosisCatalogRepository diagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetDiagnosesQuery, PageResponseDto<DiagnosisResponseDto>>
{
    public async Task<PageResponseDto<DiagnosisResponseDto>> Handle(
        GetDiagnosesQuery request,
        CancellationToken cancellationToken)
    {
        EnsureCatalogReader(currentUser);

        var (diagnoses, totalCount) = await diagnosisRepository.SearchActiveAsync(
            request.Filter.Search,
            request.Filter.Page,
            request.Filter.PageSize,
            request.Filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Filter.PageSize);

        var items = diagnoses.Select(diagnosis => new DiagnosisResponseDto
            {
                DiagnosisId = diagnosis.DiagnosisId,
                Name = diagnosis.Name
            }).ToList();
        return new PageResponseDto<DiagnosisResponseDto>(
            items,
            request.Filter.Page,
            request.Filter.PageSize,
            totalCount,
            totalPages,
            request.Filter.Page > 1,
            request.Filter.Page < totalPages);
    }

    internal static void EnsureCatalogReader(ICurrentUserService currentUser)
    {
        if (currentUser.RoleName != RoleNames.Admin &&
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La consulta del catálogo requiere el rol ADMIN o PSYCHOLOGIST.");
        }
    }
}
