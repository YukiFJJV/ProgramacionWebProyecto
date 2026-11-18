using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PsychologistRepository : IPsychologistRepository
{
    private readonly ApplicationDbContext _context;

    public PsychologistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Psychologist> ProfileQuery()
    {
        return _context.Psychologists
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .Include(p => p.Specialty);
    }

    public Task<Psychologist?> GetByIdForTenantAsync(
        Guid psychologistId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return ProfileQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.PsychologistId == psychologistId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<Psychologist?> GetByIdAsync(
        Guid psychologistId,
        CancellationToken cancellationToken = default)
    {
        return ProfileQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                psychologist => psychologist.PsychologistId == psychologistId,
                cancellationToken);
    }

    public Task<Psychologist?> GetByUserIdForTenantAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return ProfileQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.UserId == userId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> ExistsByLicenseNumberAsync(
        string licenseNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedLicense = licenseNumber.Trim();

        return _context.Psychologists
            .AsNoTracking()
            .AnyAsync(
                p => p.LicenseNumber == normalizedLicense,
                cancellationToken);
    }

    public async Task AddWithUserAsync(
        Psychologist psychologist,
        CancellationToken cancellationToken = default)
    {
        _context.Attach(psychologist.Tenant);
        _context.Attach(psychologist.Specialty);
        _context.Attach(psychologist.User.Role);
        await _context.Psychologists.AddAsync(
            psychologist,
            cancellationToken);

    }

    public async Task<(
        IReadOnlyList<Psychologist> Psychologists,
        int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? specialtyId,
        DateOnly? availableOn,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken = default)
    {
        var query = ProfileQuery()
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (specialtyId.HasValue)
        {
            query = query.Where(
                p => p.SpecialtyId == specialtyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(p =>
                p.User.FirstName.ToLower().Contains(term) ||
                p.User.LastName.ToLower().Contains(term) ||
                p.User.Email.ToLower().Contains(term) ||
                p.LicenseNumber.ToLower().Contains(term) ||
                p.Specialty.SpecialtyName.ToLower().Contains(term));
        }

        if (availableOn.HasValue)
        {
            var date = availableOn.Value;
            var day = date.ToWeekDay();
            var candidates = await query
                .Include(psychologist => psychologist.Schedules)
                .Include(psychologist => psychologist.Appointments.Where(appointment =>
                    appointment.DateAt == date &&
                    (appointment.Status == AppointmentStatus.Pending ||
                     appointment.Status == AppointmentStatus.Confirmed)))
                .ToListAsync(cancellationToken);
            var available = candidates
                .Where(psychologist => HasAvailability(psychologist, date, day));
            var totalAvailable = available.Count();
            var pageItems = OrderInMemory(available, sortBy, sortDirection)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pageItems, totalAvailable);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var orderedQuery = sortBy.ToLowerInvariant() switch
        {
            "firstname" => descending
                ? query.OrderByDescending(psychologist => psychologist.User.FirstName)
                : query.OrderBy(psychologist => psychologist.User.FirstName),
            "licensenumber" => descending
                ? query.OrderByDescending(psychologist => psychologist.LicenseNumber)
                : query.OrderBy(psychologist => psychologist.LicenseNumber),
            "specialtyname" => descending
                ? query.OrderByDescending(psychologist => psychologist.Specialty.SpecialtyName)
                : query.OrderBy(psychologist => psychologist.Specialty.SpecialtyName),
            _ => descending
                ? query.OrderByDescending(psychologist => psychologist.User.LastName)
                : query.OrderBy(psychologist => psychologist.User.LastName)
        };
        var psychologists = await orderedQuery
            .ThenBy(psychologist => psychologist.User.FirstName)
            .ThenBy(psychologist => psychologist.PsychologistId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (psychologists, totalCount);
    }

    private static bool HasAvailability(
        Psychologist psychologist,
        DateOnly date,
        WeekDay day)
    {
        foreach (var schedule in psychologist.Schedules.Where(schedule => schedule.WeekDay == day))
        {
            var availableUntil = schedule.StartsAt;
            var appointments = psychologist.Appointments
                .Where(appointment =>
                    appointment.DateAt == date &&
                    appointment.Status is AppointmentStatus.Pending or AppointmentStatus.Confirmed &&
                    appointment.StartsAt < schedule.EndsAt &&
                    schedule.StartsAt < appointment.EndsAt)
                .OrderBy(appointment => appointment.StartsAt);

            foreach (var appointment in appointments)
            {
                if (appointment.StartsAt > availableUntil)
                {
                    return true;
                }

                if (appointment.EndsAt > availableUntil)
                {
                    availableUntil = appointment.EndsAt;
                }

                if (availableUntil >= schedule.EndsAt)
                {
                    break;
                }
            }

            if (availableUntil < schedule.EndsAt)
            {
                return true;
            }
        }

        return false;
    }

    private static IOrderedEnumerable<Psychologist> OrderInMemory(
        IEnumerable<Psychologist> psychologists,
        string sortBy,
        string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var ordered = sortBy.ToLowerInvariant() switch
        {
            "firstname" => descending
                ? psychologists.OrderByDescending(psychologist => psychologist.User.FirstName, StringComparer.OrdinalIgnoreCase)
                : psychologists.OrderBy(psychologist => psychologist.User.FirstName, StringComparer.OrdinalIgnoreCase),
            "licensenumber" => descending
                ? psychologists.OrderByDescending(psychologist => psychologist.LicenseNumber, StringComparer.OrdinalIgnoreCase)
                : psychologists.OrderBy(psychologist => psychologist.LicenseNumber, StringComparer.OrdinalIgnoreCase),
            "specialtyname" => descending
                ? psychologists.OrderByDescending(psychologist => psychologist.Specialty.SpecialtyName, StringComparer.OrdinalIgnoreCase)
                : psychologists.OrderBy(psychologist => psychologist.Specialty.SpecialtyName, StringComparer.OrdinalIgnoreCase),
            _ => descending
                ? psychologists.OrderByDescending(psychologist => psychologist.User.LastName, StringComparer.OrdinalIgnoreCase)
                : psychologists.OrderBy(psychologist => psychologist.User.LastName, StringComparer.OrdinalIgnoreCase)
        };

        return ordered
            .ThenBy(psychologist => psychologist.User.FirstName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(psychologist => psychologist.PsychologistId);
    }
}