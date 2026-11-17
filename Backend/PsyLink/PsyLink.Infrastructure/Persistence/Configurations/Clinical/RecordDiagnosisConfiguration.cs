
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class RecordDiagnosisConfiguration : IEntityTypeConfiguration<RecordDiagnosis>
{
    public void Configure(EntityTypeBuilder<RecordDiagnosis> builder)
    {
        builder.ToTable("record_diagnoses");

        // Llave primaria compuesta
        builder.HasKey(rd => new { rd.RecordId, rd.DiagnosisId });

        builder.Property(rd => rd.RecordId)
            .HasColumnName("record_id");

        builder.Property(rd => rd.DiagnosisId)
            .HasColumnName("diagnosis_id");

        builder.Property(rd => rd.Notes)
            .HasColumnName("notes");

        // Relación con el expediente clínico
        builder.HasOne(rd => rd.Record)
            .WithMany(r => r.Diagnoses)
            .HasForeignKey(rd => rd.RecordId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con el catálogo de diagnósticos
        builder.HasOne(rd => rd.Diagnosis)
            .WithMany()
            .HasForeignKey(rd => rd.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rd => rd.DiagnosisId);
    }
}
