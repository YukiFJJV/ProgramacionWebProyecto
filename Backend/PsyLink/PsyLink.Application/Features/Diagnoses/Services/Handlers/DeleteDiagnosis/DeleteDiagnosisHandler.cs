using MediatR;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Diagnoses.Services.Handlers.DeleteDiagnosis;

public sealed class DeleteDiagnosisHandler(
    IDiagnosisCatalogRepository diagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteDiagnosisCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteDiagnosisCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }

        if (!await diagnosisRepository.DeleteAsync(request.DiagnosisId, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el diagnóstico {request.DiagnosisId}.");
        }

        return Unit.Value;
    }
}
