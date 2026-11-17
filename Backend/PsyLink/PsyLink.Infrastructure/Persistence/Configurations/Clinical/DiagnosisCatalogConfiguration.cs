
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class DiagnosisCatalogConfiguration : IEntityTypeConfiguration<DiagnosisCatalog>
{
    public void Configure(EntityTypeBuilder<DiagnosisCatalog> builder)
    {
        builder.ToTable("diagnosis_catalog");

        builder.HasKey(d => d.DiagnosisId);

        builder.Property(d => d.DiagnosisId)
            .HasColumnName("diagnosis_id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description");

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
    }
}
