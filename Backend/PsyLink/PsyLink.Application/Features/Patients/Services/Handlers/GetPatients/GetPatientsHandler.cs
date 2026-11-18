using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Queries;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.GetPatients;

public sealed class GetPatientsHandler(
    IPatientRepository patientRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPatientsQuery, PageResponseDto<PatientResponseDto>>
{
    public async Task<PageResponseDto<PatientResponseDto>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        Guid tenantId;
        Guid? psychologistUserId = null;
        if (currentUser.RoleName == RoleNames.Admin && currentUser.TenantId is Guid adminTenantId)
        {
            tenantId = adminTenantId;
        }
        else
        {
            var scope = PatientAccess.EnsurePsychologist(currentUser);
            tenantId = scope.TenantId;
            psychologistUserId = scope.UserId;
        }

        var (patients, totalCount) = await patientRepository.SearchByTenantAsync(
            tenantId,
            request.Search,
            request.PsychologistId,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            psychologistUserId,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        return new PageResponseDto<PatientResponseDto>
        {
            Items = patients.Select(ToResponse).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
            HasPrevious = request.Page > 1,
            HasNext = request.Page < totalPages
        };
    }

    internal static Guid EnsureCompanyAdmin(ICurrentUserService currentUser)
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere un administrador de empresa autenticado.");
        }

        return currentUser.TenantId.Value;
    }

    internal static PatientResponseDto ToResponse(PsyLink.Domain.Clinical.Patient patient) =>
        new()
        {
            PatientId = patient.PatientId,
            User = CreateUserHandler.ToResponse(patient.User),
            BirthDate = patient.BirthDate,
            Description = patient.Description,
            Age = patient.AgeOn(DateOnly.FromDateTime(DateTime.UtcNow))
        };
}
