using MediatR;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Handlers.GetDiagnoses;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.GetDiagnosisDetail;

public sealed class GetDiagnosisDetailHandler(
    IDiagnosisCatalogRepository diagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetDiagnosisDetailQuery, DiagnosisDetailResponseDto>
{
    public async Task<DiagnosisDetailResponseDto> Handle(
        GetDiagnosisDetailQuery request,
        CancellationToken cancellationToken)
    {
        GetDiagnosesHandler.EnsureCatalogReader(currentUser);
        var diagnosis = await diagnosisRepository.GetByIdAsync(
                request.DiagnosisId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        if (!diagnosis.IsActive)
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        }

        return new DiagnosisDetailResponseDto
        {
            DiagnosisId = diagnosis.DiagnosisId,
            Name = diagnosis.Name,
            Description = diagnosis.Description
        };
    }
}
