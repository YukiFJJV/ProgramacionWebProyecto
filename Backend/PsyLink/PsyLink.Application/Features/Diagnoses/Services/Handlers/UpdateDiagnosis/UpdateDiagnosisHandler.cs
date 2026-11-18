using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Diagnoses.Services.Handlers.CreateDiagnosis;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.UpdateDiagnosis;

public sealed class UpdateDiagnosisHandler(
    IDiagnosisCatalogRepository diagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateDiagnosisCommand, DiagnosisDetailResponseDto>
{
    public async Task<DiagnosisDetailResponseDto> Handle(
        UpdateDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }

        var diagnosis = await diagnosisRepository.GetByIdAsync(
                request.DiagnosisId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        if (!diagnosis.IsActive)
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        }

        diagnosis.Name = request.Diagnosis.Name.Trim();
        diagnosis.Description = CreateDiagnosisHandler.NormalizeDescription(request.Diagnosis.Description);
        await diagnosisRepository.UpdateAsync(diagnosis, cancellationToken);
        return CreateDiagnosisHandler.ToResponse(diagnosis);
    }
}
