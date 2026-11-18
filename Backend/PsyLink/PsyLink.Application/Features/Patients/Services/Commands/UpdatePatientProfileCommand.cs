using MediatR;
using PsyLink.Application.Features.Patients.DTOs;

namespace PsyLink.Application.Features.Diagnoses.Services.Commands;

public sealed record UpdatePatientProfileCommand(
    Guid PatientId,
    string? FirstName,
    bool FirstNameSpecified,
    string? LastName,
    bool LastNameSpecified,
    string? Email,
    bool EmailSpecified,
    string? PhoneNumber,
    bool PhoneNumberSpecified,
    DateOnly? BirthDate,
    bool BirthDateSpecified,
    string? Description,
    bool DescriptionSpecified) : ICommand<PatientResponseDto>;
