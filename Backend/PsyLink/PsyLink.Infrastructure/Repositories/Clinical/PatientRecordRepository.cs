using Microsoft.EntityFrameworkCore;
using Npgsql;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class PatientRecordRepository : IPatientRecordRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<PatientRecord?> GetByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.PatientRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.PatientId == patientId &&
                     r.TenantId == tenantId,
                cancellationToken);
    }

    public Task<PatientRecord?> GetByPatientIdWithDetailsAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.PatientRecords
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.Appointment)
            .Include(r => r.Diagnoses)
                .ThenInclude(d => d.Diagnosis)
            .Include(r => r.Prescriptions)
                .ThenInclude(p => p.Medication)
                    .ThenInclude(m => m.Laboratory)
            .Include(r => r.Prescriptions)
                .ThenInclude(p => p.Medication)
                    .ThenInclude(m => m.Ingredients)
                        .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(
                r => r.PatientId == patientId &&
                     r.TenantId == tenantId,
                cancellationToken);
    }

    public Task<PatientRecord?> GetByRecordIdForTenantAsync(
        Guid recordId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.PatientRecords
            .FirstOrDefaultAsync(
                r => r.RecordId == recordId &&
                     r.TenantId == tenantId,
                cancellationToken);
    }

    public Task<bool> ExistsByPatientIdAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.PatientRecords
            .AsNoTracking()
            .AnyAsync(
                r => r.PatientId == patientId &&
                     r.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<bool> AddIfPatientHasNoRecordAsync(
        PatientRecord record,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var exists = await _context.PatientRecords
                .AnyAsync(
                    r => r.PatientId == record.PatientId,
                    cancellationToken);

            if (exists)
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await _context.PatientRecords.AddAsync(
                record,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (PostgresException ex)
            when (ex.SqlState == "23505" ||
                  ex.SqlState == "40001" ||
                  ex.SqlState == "40P01")
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public async Task UpdateAsync(
        PatientRecord record,
        CancellationToken cancellationToken = default)
    {
        _context.PatientRecords.Update(record);

    }
}