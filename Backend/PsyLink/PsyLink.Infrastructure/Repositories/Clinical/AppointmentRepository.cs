using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Appointment> DisplayQuery()
    {
        return _context.Appointments
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Role)
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Phone)
            .Include(a => a.Psychologist)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Phone)
            .Include(a => a.Psychologist)
                .ThenInclude(p => p.Specialty);
    }

    public Task<Appointment?> GetByIdForTenantAsync(
        Guid appointmentId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Se mantiene tracked porque algunos handlers modifican su estado.
        return DisplayQuery()
            .FirstOrDefaultAsync(
                a => a.AppointmentId == appointmentId
                     && a.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> HasOverlapAsync(
        Guid psychologistId,
        DateOnly dateAt,
        TimeOnly startsAt,
        TimeOnly endsAt,
        Guid? excludingAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.PsychologistId == psychologistId &&
                a.DateAt == dateAt &&
                a.Status != AppointmentStatus.Cancelled &&
                a.StartsAt < endsAt &&
                a.EndsAt > startsAt);

        if (excludingAppointmentId.HasValue)
        {
            query = query.Where(
                a => a.AppointmentId != excludingAppointmentId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>>
        GetByPsychologistAndDateRangeAsync(
            Guid psychologistId,
            DateOnly dateFrom,
            DateOnly dateTo,
            CancellationToken cancellationToken = default)
    {
        return await DisplayQuery()
            .AsNoTracking()
            .Where(a =>
                a.PsychologistId == psychologistId &&
                a.DateAt >= dateFrom &&
                a.DateAt <= dateTo &&
                a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.DateAt)
            .ThenBy(a => a.StartsAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(
        IReadOnlyList<Appointment> Items,
        int TotalCount)> GetByTenantWithPatientSearchAsync(
            Guid tenantId,
            string? patientSearch,
            Guid? patientId,
            Guid? psychologistId,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            int page,
            int pageSize,
            bool descending,
            CancellationToken cancellationToken = default)
    {
        var query = DisplayQuery()
            .AsNoTracking()
            .Where(a =>
                a.TenantId == tenantId &&
                a.Status != AppointmentStatus.Cancelled);

        if (patientId.HasValue)
        {
            query = query.Where(
                a => a.PatientId == patientId.Value);
        }

        if (psychologistId.HasValue)
        {
            query = query.Where(
                a => a.PsychologistId == psychologistId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(
                a => a.DateAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(
                a => a.DateAt <= dateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(patientSearch))
        {
            var term = patientSearch.Trim().ToLower();

            query = query.Where(a =>
                a.Patient.User.FirstName.ToLower().Contains(term) ||
                a.Patient.User.LastName.ToLower().Contains(term) ||
                a.Patient.User.Email.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = descending
            ? query.OrderByDescending(a => a.DateAt)
                .ThenByDescending(a => a.StartsAt)
                .ThenByDescending(a => a.AppointmentId)
            : query.OrderBy(a => a.DateAt)
                .ThenBy(a => a.StartsAt)
                .ThenBy(a => a.AppointmentId);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Appointment>>
        GetByPatientAndTenantAsync(
            Guid patientId,
            Guid tenantId,
            CancellationToken cancellationToken = default)
    {
        return await DisplayQuery()
            .AsNoTracking()
            .Where(a =>
                a.PatientId == patientId &&
                a.TenantId == tenantId)
            .OrderByDescending(a => a.DateAt)
            .ThenByDescending(a => a.StartsAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveCareRelationshipAsync(
        Guid patientId,
        Guid psychologistUserId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var latestAppointment = await _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.PatientId == patientId &&
                a.TenantId == tenantId &&
                a.Status != AppointmentStatus.Cancelled)
            .OrderByDescending(a => a.DateAt)
            .ThenByDescending(a => a.StartsAt)
            .Select(a => new
            {
                a.Psychologist.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        return latestAppointment?.UserId == psychologistUserId;
    }

    public async Task<bool> AddIfNoOverlapAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        await AcquireScheduleLockAsync(
            appointment.PsychologistId,
            appointment.DateAt,
            cancellationToken);

        var hasOverlap = await HasOverlapAsync(
            appointment.PsychologistId,
            appointment.DateAt,
            appointment.StartsAt,
            appointment.EndsAt,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await _context.Appointments.AddAsync(
            appointment,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateIfNoOverlapAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        await AcquireScheduleLockAsync(
            appointment.PsychologistId,
            appointment.DateAt,
            cancellationToken);

        var hasOverlap = await HasOverlapAsync(
            appointment.PsychologistId,
            appointment.DateAt,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.AppointmentId,
            cancellationToken);

        if (hasOverlap)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        if (_context.Entry(appointment).State == EntityState.Detached)
        {
            _context.Appointments.Update(appointment);
        }

        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    private async Task AcquireScheduleLockAsync(
        Guid psychologistId,
        DateOnly dateAt,
        CancellationToken cancellationToken)
    {
        var lockKey = $"{psychologistId}:{dateAt:yyyy-MM-dd}";

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))",
            cancellationToken);
    }
}