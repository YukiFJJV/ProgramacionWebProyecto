using MediatR;
using PsyLink.Application.Features.Medications.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;

namespace PsyLink.Application.Features.Medications.Services.Commands;

public sealed record CreateMedicationCommand(MedicationCreateRequestDto Medication)
    : ICommand<MedicationResponseDto>;
