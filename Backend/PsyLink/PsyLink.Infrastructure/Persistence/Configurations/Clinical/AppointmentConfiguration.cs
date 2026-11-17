
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", table =>
        {
            table.HasCheckConstraint(
                "ck_appointments_valid_time",
                "ends_at > starts_at");

            table.HasCheckConstraint(
                "ck_appointments_reschedule_count",
                "reschedule_count >= 0");
        });

        builder.HasKey(a => a.AppointmentId);

        builder.Property(a => a.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(a => a.PsychologistId)
            .HasColumnName("psychologist_id")
            .IsRequired();

        builder.Property(a => a.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(a => a.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(a => a.DateAt)
            .HasColumnName("date_at")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(a => a.StartsAt)
            .HasColumnName("starts_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(a => a.EndsAt)
            .HasColumnName("ends_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.RescheduleCount)
            .HasColumnName("reschedule_count")
            .IsRequired();

        builder.HasOne(a => a.Psychologist)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PsychologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new
        {
            a.PsychologistId,
            a.DateAt
        });

        builder.HasIndex(a => new
        {
            a.PatientId,
            a.DateAt
        });

        builder.HasIndex(a => a.TenantId);
    }
}
