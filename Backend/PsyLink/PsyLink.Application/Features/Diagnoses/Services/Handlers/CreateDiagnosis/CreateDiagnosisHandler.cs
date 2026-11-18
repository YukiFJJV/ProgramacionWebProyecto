using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.CreateDiagnosis;

public sealed class CreateDiagnosisHandler(
    IDiagnosisCatalogRepository diagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateDiagnosisCommand, DiagnosisDetailResponseDto>
{
    public async Task<DiagnosisDetailResponseDto> Handle(
        CreateDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        EnsureAdmin();
        var diagnosis = new DiagnosisCatalog
        {
            DiagnosisId = Guid.NewGuid(),
            Name = request.Diagnosis.Name.Trim(),
            Description = NormalizeDescription(request.Diagnosis.Description)
        };
        await diagnosisRepository.AddAsync(diagnosis, cancellationToken);
        return ToResponse(diagnosis);
    }

    internal static DiagnosisDetailResponseDto ToResponse(DiagnosisCatalog diagnosis) =>
        new()
        {
            DiagnosisId = diagnosis.DiagnosisId,
            Name = diagnosis.Name,
            Description = diagnosis.Description
        };

    internal static string? NormalizeDescription(string? description)
    {
        var normalized = description?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }

    private void EnsureAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }
    }
}
