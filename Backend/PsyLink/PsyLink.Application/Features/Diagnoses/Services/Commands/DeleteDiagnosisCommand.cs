using MediatR;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record DeleteDiagnosisCommand(Guid DiagnosisId) : ICommand<Unit>;
