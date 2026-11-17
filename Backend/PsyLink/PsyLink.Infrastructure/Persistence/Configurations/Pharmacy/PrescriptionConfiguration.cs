
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("prescriptions", table =>
        {
            table.HasCheckConstraint(
                "ck_prescriptions_quantity",
                "quantity IS NULL OR quantity > 0");
        });

        builder.HasKey(p => p.PrescriptionId);

        builder.Property(p => p.PrescriptionId)
            .HasColumnName("prescription_id");

        builder.Property(p => p.RecordId)
            .HasColumnName("record_id")
            .IsRequired();

        builder.Property(p => p.MedicationId)
            .HasColumnName("medication_id")
            .IsRequired();

        builder.Property(p => p.Dosage)
            .HasColumnName("dosage");

        builder.Property(p => p.Quantity)
            .HasColumnName("quantity");

        builder.HasOne(p => p.Record)
            .WithMany(r => r.Prescriptions)
            .HasForeignKey(p => p.RecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Medication)
            .WithMany()
            .HasForeignKey(p => p.MedicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.RecordId);
        builder.HasIndex(p => p.MedicationId);
    }
}
