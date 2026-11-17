using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Appointments.Services.Queries;

public sealed record GetPsychologistAppointmentsQuery(AppointmentQueryDto Filter)
    : IRequest<PageResponseDto<AppointmentResponseDto>>;
