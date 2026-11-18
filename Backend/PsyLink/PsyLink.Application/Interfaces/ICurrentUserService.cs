namespace PsyLink.Application.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    Guid UserId { get; }

    Guid? TenantId { get; }

    string? RoleName { get; }
}
