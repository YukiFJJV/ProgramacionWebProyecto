using PsyLink.Domain.Identity;

namespace PsyLink.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByIdForTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdForTenantForUpdateAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<User> Items, int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? roleId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> GetAssignableRolesAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        Guid excludingUserId,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

}
