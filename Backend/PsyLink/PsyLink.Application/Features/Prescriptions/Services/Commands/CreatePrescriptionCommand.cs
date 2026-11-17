using MediatR;
using PsyLink.Application.Features.Prescriptions.DTOs;

namespace PsyLink.Application.Features.Prescriptions.Services.Commands;

public sealed record CreatePrescriptionCommand(Guid RecordId, PrescriptionRequestDto Prescription)
    : IRequest<PrescriptionResponseDto>;
