using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;

namespace PsyLink.Application.Features.Appointments.Services.Commands;

public record RescheduleAppointmentCommand(
    Guid AppointmentId,
    AppointmentUpdateRequestDto Update)
    : ICommand<AppointmentResponseDto>;
