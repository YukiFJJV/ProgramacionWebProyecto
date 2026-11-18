using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Patients.Services.Commands;

public record RegisterPatientCommand(PatientCreateRequestDto Patient) : ICommand<PatientResponseDto>;
