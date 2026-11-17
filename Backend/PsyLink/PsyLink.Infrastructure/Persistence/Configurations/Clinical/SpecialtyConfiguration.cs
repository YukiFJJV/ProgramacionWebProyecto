
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.ToTable("specialties");

        builder.HasKey(s => s.SpecialtyId);

        builder.Property(s => s.SpecialtyId)
            .HasColumnName("specialty_id");

        builder.Property(s => s.SpecialtyName)
            .HasColumnName("specialty_name")
            .IsRequired();

        builder.HasIndex(s => s.SpecialtyName)
            .IsUnique();
    }
}
