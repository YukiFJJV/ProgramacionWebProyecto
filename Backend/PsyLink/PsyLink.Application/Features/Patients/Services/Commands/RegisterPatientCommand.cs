using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public record RegisterPatientCommand(PatientCreateRequestDto Patient) : ICommand<PatientResponseDto>;
