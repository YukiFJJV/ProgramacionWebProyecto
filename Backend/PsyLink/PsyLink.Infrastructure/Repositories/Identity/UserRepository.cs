
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

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
