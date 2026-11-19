using PsyLink.Application.Interfaces;

namespace PsyLink.API;

public sealed class PublicRegistrationSettings(Guid? defaultTenantId)
    : IPublicRegistrationSettings
{
    public Guid? DefaultTenantId { get; } = defaultTenantId;
}
