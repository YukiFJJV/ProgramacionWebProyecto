using PsyLink.Application.Interfaces;

namespace PsyLink.API;

public sealed class RoleSettings(Guid patientRoleId) : IRoleSettings
{
    public Guid PatientRoleId { get; } = patientRoleId;
}
