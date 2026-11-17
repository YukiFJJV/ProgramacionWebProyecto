
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class DiagnosisStatusConfiguration : IEntityTypeConfiguration<DiagnosisStatus>
{
    public void Configure(EntityTypeBuilder<DiagnosisStatus> builder)
    {
        builder.ToTable("diagnosis_statuses");

        builder.HasKey(s => s.StatusId);

        builder.Property(s => s.StatusId)
            .HasColumnName("status_id");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .IsRequired();
    }
}
