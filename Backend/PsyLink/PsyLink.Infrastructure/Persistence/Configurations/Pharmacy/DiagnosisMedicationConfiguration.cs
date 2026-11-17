
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class DiagnosisMedicationConfiguration
    : IEntityTypeConfiguration<DiagnosisMedication>
{
    public void Configure(EntityTypeBuilder<DiagnosisMedication> builder)
    {
        builder.ToTable("diagnosis_medications");

        builder.HasKey(dm => new
        {
            dm.DiagnosisId,
            dm.MedicationId
        });

        builder.Property(dm => dm.DiagnosisId)
            .HasColumnName("diagnosis_id");

        builder.Property(dm => dm.MedicationId)
            .HasColumnName("medication_id");

        builder.HasOne(dm => dm.Diagnosis)
            .WithMany(d => d.SuggestedMedications)
            .HasForeignKey(dm => dm.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dm => dm.Medication)
            .WithMany()
            .HasForeignKey(dm => dm.MedicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dm => dm.MedicationId);
    }
}
