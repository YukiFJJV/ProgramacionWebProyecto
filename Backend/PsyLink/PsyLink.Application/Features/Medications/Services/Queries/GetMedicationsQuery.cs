using MediatR;
using PsyLink.Application.Features.Medications.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Medications.Services.Queries;

public sealed record GetMedicationsQuery(MedicationQueryDto Filter)
    : IQuery<PageResponseDto<MedicationResponseDto>>;
