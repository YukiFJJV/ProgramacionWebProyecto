using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;

namespace PsyLink.Application.Features.PatientRecords.Services.Queries;

public sealed record GetRecordByIdQuery(Guid RecordId) : IQuery<RecordDetailResponseDto>;
