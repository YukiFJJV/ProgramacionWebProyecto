using PsyLink.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class PatientRecordConfiguration : IEntityTypeConfiguration<PatientRecord>
{
    public void Configure(EntityTypeBuilder<PatientRecord> builder)
    {
        builder.ToTable("patient_records", table =>
        {
            table.HasCheckConstraint(
                "ck_patient_records_risk_score",
                "risk_score >= 0 AND risk_score <= 3");
        });

        builder.HasKey(r => r.RecordId);

        builder.Property(r => r.RecordId)
            .HasColumnName("record_id");

        builder.Property(r => r.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(r => r.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(r => r.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(r => r.RiskScore)
            .HasColumnName("risk_score")
            .IsRequired();

        builder.HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Appointment)
            .WithMany()
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.PatientId)
            .IsUnique();

        builder.HasIndex(r => r.AppointmentId);

        builder.HasIndex(r => r.TenantId);
    }
}
