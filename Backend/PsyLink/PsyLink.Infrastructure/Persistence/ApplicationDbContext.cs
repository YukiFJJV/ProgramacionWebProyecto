
using Microsoft.EntityFrameworkCore;

using PsyLink.Domain.Identity;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Pharmacy;
using PsyLink.Domain.Billing;
using PsyLink.Application.Common.Interfaces;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Common;
using PsyLink.Infrastructure.Persistence.Entities.Clinical;

namespace PsyLink.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly Guid? _tenantId;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser)
        : base(options)
    {
        _tenantId = currentUser.TenantId;
    }

    // IDENTITY
    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<PhoneNumber> PhoneNumbers => Set<PhoneNumber>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subplan> Subplans => Set<Subplan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    // CLINICAL
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Psychologist> Psychologists => Set<Psychologist>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentSessionNotes> AppointmentSessionNotes =>
        Set<AppointmentSessionNotes>();
    public DbSet<PatientRecord> PatientRecords => Set<PatientRecord>();
    public DbSet<DiagnosisCatalog> DiagnosisCatalog => Set<DiagnosisCatalog>();
    public DbSet<PatientDiagnosis> PatientDiagnoses => Set<PatientDiagnosis>();
    public DbSet<RecordDiagnosis> RecordDiagnoses => Set<RecordDiagnosis>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();

    // PHARMACY
    public DbSet<ActiveIngredient> ActiveIngredients => Set<ActiveIngredient>();
    public DbSet<DiagnosisMedication> DiagnosisMedications => Set<DiagnosisMedication>();
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationIngredient> MedicationIngredients => Set<MedicationIngredient>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    // BILLING
    public DbSet<BillDetail> BillDetails => Set<BillDetail>();
    public DbSet<FiscalAuthorization> FiscalAuthorizations => Set<FiscalAuthorization>();
    public DbSet<PaymentHistory> PaymentHistories => Set<PaymentHistory>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(entity =>
                _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<User>()
            .HasQueryFilter(entity =>
                _tenantId == null ||
                entity.TenantId == null ||
                entity.TenantId == _tenantId);
        modelBuilder.Entity<Patient>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<Psychologist>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<AppointmentSessionNotes>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<PatientRecord>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<WorkSchedule>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<Subscription>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<BillDetail>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
        modelBuilder.Entity<FiscalAuthorization>()
            .HasQueryFilter(entity => _tenantId == null || entity.TenantId == _tenantId);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AssignTenantId();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        AssignTenantId();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AssignTenantId()
    {
        if (_tenantId is null)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added)
            {
                continue;
            }

            if (entry.Entity is ITenantScoped scoped &&
                scoped.TenantId == Guid.Empty)
            {
                scoped.TenantId = _tenantId.Value;
            }

            if (entry.Entity is User user &&
                user.TenantId is null)
            {
                user.TenantId = _tenantId;
            }
        }
    }
}
