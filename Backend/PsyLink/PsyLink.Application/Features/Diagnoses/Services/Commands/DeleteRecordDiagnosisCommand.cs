using MediatR;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record DeleteRecordDiagnosisCommand(Guid RecordId, Guid DiagnosisId) : ICommand<Unit>;
