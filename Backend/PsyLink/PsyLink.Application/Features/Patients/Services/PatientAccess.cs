using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services;

internal static class PatientAccess
{
    public static (Guid TenantId, Guid UserId) EnsurePsychologist(
        ICurrentUserService currentUser)
    {
        if (currentUser.RoleName != RoleNames.Psychologist ||
            currentUser.TenantId is not Guid tenantId ||
            currentUser.UserId == Guid.Empty)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere un psicólogo autenticado.");
        }

        return (tenantId, currentUser.UserId);
    }
}
