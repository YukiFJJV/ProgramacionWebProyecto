using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;

namespace PsyLink.Application.Features.Appointments.Services.Commands;

public record CreateAppointmentCommand(AppointmentCreateRequestDto Appointment)
    : ICommand<AppointmentResponseDto>;
