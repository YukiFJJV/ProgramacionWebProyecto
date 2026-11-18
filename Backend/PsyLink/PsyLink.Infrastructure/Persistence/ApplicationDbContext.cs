
using Microsoft.EntityFrameworkCore;

using PsyLink.Domain.Identity;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Pharmacy;
using PsyLink.Domain.Billing;
using PsyLink.Application.Common.Interfaces;

namespace PsyLink.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
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
    }
}
