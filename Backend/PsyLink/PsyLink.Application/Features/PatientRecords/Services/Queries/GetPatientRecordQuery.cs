using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;

namespace PsyLink.Application.Features.PatientRecords.Services.Queries;

public sealed record GetPatientRecordQuery(Guid PatientId) : IQuery<RecordDetailResponseDto>;
