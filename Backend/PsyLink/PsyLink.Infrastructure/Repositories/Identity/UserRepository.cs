
using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Identity;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .Include(u => u.Phone)
            .Include(u => u.Tenant)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Email.ToLower() == normalizedEmail,
                cancellationToken);
    }

    public Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Include(u => u.Role)
            .Include(u => u.Phone)
            .Include(u => u.Tenant)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.UserId == userId,
                cancellationToken);
    }

    public Task<User?> GetByIdForTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Include(user => user.Role)
            .Include(user => user.Phone)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.UserId == userId && user.TenantId == tenantId,
                cancellationToken);
    }

    public Task<User?> GetByIdForTenantForUpdateAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Include(user => user.Role)
            .Include(user => user.Phone)
            .FirstOrDefaultAsync(
                user => user.UserId == userId && user.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? roleId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default)
    {
        var usersQuery = _context.Users
            .Where(user => user.TenantId == tenantId);

        if (roleId.HasValue)
        {
            usersQuery = usersQuery.Where(user => user.RoleId == roleId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            usersQuery = usersQuery.Where(user =>
                user.FirstName.ToLower().Contains(searchTerm) ||
                user.LastName.ToLower().Contains(searchTerm) ||
                user.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = await usersQuery.CountAsync(cancellationToken);
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var orderedQuery = sortBy.ToLowerInvariant() switch
        {
            "lastname" => descending
                ? usersQuery.OrderByDescending(user => user.LastName)
                : usersQuery.OrderBy(user => user.LastName),
            "email" => descending
                ? usersQuery.OrderByDescending(user => user.Email)
                : usersQuery.OrderBy(user => user.Email),
            "role" => descending
                ? usersQuery.OrderByDescending(user => user.Role.Name)
                : usersQuery.OrderBy(user => user.Role.Name),
            _ => descending
                ? usersQuery.OrderByDescending(user => user.FirstName)
                : usersQuery.OrderBy(user => user.FirstName)
        };
        var users = await orderedQuery
            .ThenBy(user => user.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(user => user.Role)
            .Include(user => user.Phone)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public Task<Role?> GetRoleByIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.RoleId == roleId,
                cancellationToken);
    }

    public Task<Role?> GetRoleByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = roleName.Trim().ToUpper();

        return _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.Name.ToUpper() == normalizedName,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAssignableRolesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(r => r.Name != RoleNames.SuperAdmin)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return _context.Users
            .AsNoTracking()
            .AnyAsync(
                u => u.Email.ToLower() == normalizedEmail,
                cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        Guid excludingUserId,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return _context.Users
            .AsNoTracking()
            .AnyAsync(
                user => user.UserId != excludingUserId &&
                    user.Email.ToLower() == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);

    }

}
