using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class WorkScheduleRepository : IWorkScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public WorkScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<WorkSchedule?> GetByIdForTenantAsync(
        Guid scheduleId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.WorkSchedules
            .FirstOrDefaultAsync(
                s => s.ScheduleId == scheduleId &&
                     s.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<WorkSchedule>> GetByPsychologistAsync(
        Guid psychologistId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WorkSchedules
            .AsNoTracking()
            .Where(s => s.PsychologistId == psychologistId &&
                        s.TenantId == tenantId)
            .OrderBy(s => s.WeekDay)
            .ThenBy(s => s.StartsAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkSchedule>> GetOverlappingAsync(
        Guid psychologistId,
        Guid tenantId,
        WeekDay weekDay,
        TimeOnly startsAt,
        TimeOnly endsAt,
        Guid? excludingScheduleId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.WorkSchedules
            .AsNoTracking()
            .Where(s =>
                s.PsychologistId == psychologistId &&
                s.TenantId == tenantId &&
                s.WeekDay == weekDay &&
                s.StartsAt < endsAt &&
                startsAt < s.EndsAt);

        if (excludingScheduleId.HasValue)
        {
            query = query.Where(
                s => s.ScheduleId != excludingScheduleId.Value);
        }

        return await query
            .OrderBy(s => s.StartsAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AddIfNoOverlapAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var hasOverlap = await _context.WorkSchedules
                .AnyAsync(s =>
                    s.PsychologistId == schedule.PsychologistId &&
                    s.TenantId == schedule.TenantId &&
                    s.WeekDay == schedule.WeekDay &&
                    s.StartsAt < schedule.EndsAt &&
                    schedule.StartsAt < s.EndsAt,
                    cancellationToken);

            if (hasOverlap)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await _context.WorkSchedules.AddAsync(
                schedule,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (PostgresException ex)
            when (ex.SqlState == "40001" || ex.SqlState == "40P01")
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public async Task<bool> UpdateIfNoOverlapAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var hasOverlap = await _context.WorkSchedules
                .AnyAsync(s =>
                    s.PsychologistId == schedule.PsychologistId &&
                    s.TenantId == schedule.TenantId &&
                    s.WeekDay == schedule.WeekDay &&
                    s.ScheduleId != schedule.ScheduleId &&
                    s.StartsAt < schedule.EndsAt &&
                    schedule.StartsAt < s.EndsAt,
                    cancellationToken);

            if (hasOverlap)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            _context.WorkSchedules.Update(schedule);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (PostgresException ex)
            when (ex.SqlState == "40001" || ex.SqlState == "40P01")
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }
}