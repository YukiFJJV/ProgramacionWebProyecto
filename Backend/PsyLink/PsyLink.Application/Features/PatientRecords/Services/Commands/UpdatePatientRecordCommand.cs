using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;

namespace PsyLink.Application.Features.PatientRecords.Services.Commands;

public sealed record UpdatePatientRecordCommand(Guid RecordId, RecordUpdateRequestDto Update)
    : ICommand<RecordDetailResponseDto>;
