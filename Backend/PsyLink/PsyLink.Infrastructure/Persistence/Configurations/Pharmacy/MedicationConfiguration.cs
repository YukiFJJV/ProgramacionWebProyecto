
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("medications", table =>
        {
            table.HasCheckConstraint(
                "ck_medications_content",
                "content > 0");
        });

        builder.HasKey(m => m.MedicationId);

        builder.Property(m => m.MedicationId)
            .HasColumnName("medication_id");

        builder.Property(m => m.LaboratoryId)
            .HasColumnName("laboratory_id");

        builder.Property(m => m.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(m => m.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(m => m.Presentation)
            .HasColumnName("presentation")
            .IsRequired();

        builder.HasOne(m => m.Laboratory)
            .WithMany()
            .HasForeignKey(m => m.LaboratoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.LaboratoryId);
    }
}
