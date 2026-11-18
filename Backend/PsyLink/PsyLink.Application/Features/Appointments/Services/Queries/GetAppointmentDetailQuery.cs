using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;

namespace PsyLink.Application.Features.Appointments.Services.Queries;

public sealed record GetAppointmentDetailQuery(Guid AppointmentId)
    : IQuery<AppointmentDetailResponseDto>;
