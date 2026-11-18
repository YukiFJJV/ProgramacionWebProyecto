using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Commands;
using PsyLink.Application.Features.Patients.Services.Handlers.GetPatients;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.UpdatePatientProfile;

public sealed class UpdatePatientProfileHandler(
    IPatientRepository patientRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdatePatientProfileCommand, PatientResponseDto>
{
    public async Task<PatientResponseDto> Handle(
        UpdatePatientProfileCommand request,
        CancellationToken cancellationToken)
    {
        var scope = PatientAccess.EnsurePsychologist(currentUser);
        var patient = await patientRepository.GetByIdForPsychologistForUpdateAsync(
            request.PatientId,
            scope.TenantId,
            scope.UserId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {request.PatientId}.");
        var user = patient.User;

        if (request.EmailSpecified)
        {
            if (await userRepository.ExistsByEmailAsync(
                    request.Email!,
                    user.UserId,
                    cancellationToken))
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Ya existe una cuenta con ese correo electrónico.");
            }

            user.Email = request.Email!;
        }

        if (request.FirstNameSpecified)
        {
            user.FirstName = request.FirstName!;
        }

        if (request.LastNameSpecified)
        {
            user.LastName = request.LastName!;
        }

        if (request.PhoneNumberSpecified)
        {
            if (request.PhoneNumber is null)
            {
                user.PhoneId = null;
                user.Phone = null;
            }
            else if (user.Phone is null)
            {
                user.Phone = new PhoneNumber
                {
                    PhoneId = Guid.NewGuid(),
                    Number = request.PhoneNumber
                };
                user.PhoneId = user.Phone.PhoneId;
            }
            else
            {
                user.Phone.Number = request.PhoneNumber;
            }
        }

        if (request.BirthDateSpecified)
        {
            patient.BirthDate = request.BirthDate!.Value;
        }

        if (request.DescriptionSpecified)
        {
            patient.Description = request.Description;
        }

        await patientRepository.UpdateAsync(patient, cancellationToken);
        return GetPatientsHandler.ToResponse(patient);
    }
}
