namespace PsyLink.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }

    Guid? TenantId { get; }

    string? RoleName { get; }
}
