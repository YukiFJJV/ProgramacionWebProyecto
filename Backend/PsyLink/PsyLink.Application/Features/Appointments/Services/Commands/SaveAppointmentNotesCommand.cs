using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;

namespace PsyLink.Application.Features.Appointments.Services.Commands;

public sealed record SaveAppointmentNotesCommand(
    Guid AppointmentId,
    AppointmentSessionNotesRequestDto Notes)
    : IRequest<AppointmentSessionNotesResponseDto>;
