using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Patient?> GetByIdForTenantAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .FirstOrDefaultAsync(
                p => p.PatientId == patientId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<Patient?> GetByIdForPsychologistAsync(
        Guid patientId,
        Guid tenantId,
        Guid psychologistUserId,
        CancellationToken cancellationToken = default)
    {
        return BuildPsychologistPatientQuery(patientId, tenantId, psychologistUserId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Patient?> GetByIdForPsychologistForUpdateAsync(
        Guid patientId,
        Guid tenantId,
        Guid psychologistUserId,
        CancellationToken cancellationToken = default)
    {
        return BuildPsychologistPatientQuery(patientId, tenantId, psychologistUserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Patient?> GetByUserIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.Role)
            .Include(p => p.User)
                .ThenInclude(u => u.Phone)
            .FirstOrDefaultAsync(
                p => p.UserId == userId
                     && p.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Patients
            .AsNoTracking()
            .AnyAsync(
                p => p.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);

    }

    public async Task AddWithUserAsync(
        Patient patient,
        User user,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        patient.User = user;
        await _context.Patients.AddAsync(patient, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {

    }

    public async Task<(IReadOnlyList<Patient> Patients, int TotalCount)> SearchByTenantAsync(
        Guid tenantId,
        string? search,
        Guid? psychologistId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        Guid? psychologistUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Patients
            .Where(patient => patient.TenantId == tenantId);

        if (psychologistId.HasValue)
        {
            query = query.Where(patient => patient.Appointments.Any(appointment =>
                appointment.TenantId == tenantId &&
                appointment.PsychologistId == psychologistId.Value));
        }

        if (psychologistUserId.HasValue)
        {
            query = query.Where(patient => patient.Appointments.Any(appointment =>
                appointment.TenantId == tenantId &&
                appointment.Psychologist.UserId == psychologistUserId.Value));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            query = query.Where(patient =>
                patient.User.FirstName.ToLower().Contains(searchTerm) ||
                patient.User.LastName.ToLower().Contains(searchTerm) ||
                patient.User.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
        var orderedQuery = sortBy.ToLowerInvariant() switch
        {
            "lastname" => descending
                ? query.OrderByDescending(patient => patient.User.LastName)
                : query.OrderBy(patient => patient.User.LastName),
            "email" => descending
                ? query.OrderByDescending(patient => patient.User.Email)
                : query.OrderBy(patient => patient.User.Email),
            "birthdate" => descending
                ? query.OrderByDescending(patient => patient.BirthDate)
                : query.OrderBy(patient => patient.BirthDate),
            "patientid" => descending
                ? query.OrderByDescending(patient => patient.PatientId)
                : query.OrderBy(patient => patient.PatientId),
            _ => descending
                ? query.OrderByDescending(patient => patient.User.FirstName)
                : query.OrderBy(patient => patient.User.FirstName)
        };
        var patients = await orderedQuery
            .ThenBy(patient => patient.PatientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(patient => patient.User)
                .ThenInclude(user => user.Role)
            .Include(patient => patient.User)
                .ThenInclude(user => user.Phone)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (patients, totalCount);
    }

    private IQueryable<Patient> BuildPsychologistPatientQuery(
        Guid patientId,
        Guid tenantId,
        Guid psychologistUserId) =>
        _context.Patients
            .Include(patient => patient.User)
                .ThenInclude(user => user.Role)
            .Include(patient => patient.User)
                .ThenInclude(user => user.Phone)
            .Where(patient =>
                patient.PatientId == patientId &&
                patient.TenantId == tenantId &&
                patient.Appointments.Any(appointment =>
                    appointment.TenantId == tenantId &&
                    appointment.Psychologist.UserId == psychologistUserId));
}