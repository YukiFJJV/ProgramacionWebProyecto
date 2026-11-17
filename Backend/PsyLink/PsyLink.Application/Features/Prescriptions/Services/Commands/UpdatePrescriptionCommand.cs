using MediatR;
using PsyLink.Application.Features.Prescriptions.DTOs;

namespace PsyLink.Application.Features.Prescriptions.Services.Commands;

public sealed record UpdatePrescriptionCommand(
    Guid RecordId,
    Guid PrescriptionId,
    PrescriptionUpdateRequestDto Update)
    : IRequest<PrescriptionResponseDto>;
