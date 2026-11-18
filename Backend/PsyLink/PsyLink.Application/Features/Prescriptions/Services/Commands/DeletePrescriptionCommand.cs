using MediatR;

namespace PsyLink.Application.Features.Prescriptions.Services.Commands;

public sealed record DeletePrescriptionCommand(Guid RecordId, Guid PrescriptionId) : ICommand<Unit>;
