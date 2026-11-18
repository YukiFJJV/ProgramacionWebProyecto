using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.GetDiagnosisStatuses;

public sealed class GetDiagnosisStatusesHandler(
    IPatientDiagnosisRepository patientDiagnosisRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetDiagnosisStatusesQuery, IReadOnlyList<DiagnosisStatusResponseDto>>
{
    public async Task<IReadOnlyList<DiagnosisStatusResponseDto>> Handle(
        GetDiagnosisStatusesQuery request,
        CancellationToken cancellationToken)
    {
        PatientAccess.EnsurePsychologist(currentUser);
        var statuses = await patientDiagnosisRepository.GetStatusesAsync(cancellationToken);
        return statuses.Select(status => new DiagnosisStatusResponseDto
        {
            StatusId = status.StatusId,
            Name = status.Name
        }).ToList();
    }
}
