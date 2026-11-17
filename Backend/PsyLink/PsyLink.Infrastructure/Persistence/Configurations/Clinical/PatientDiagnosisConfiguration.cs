
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class PatientDiagnosisConfiguration : IEntityTypeConfiguration<PatientDiagnosis>
{
    public void Configure(EntityTypeBuilder<PatientDiagnosis> builder)
    {
        builder.ToTable("patient_diagnoses");

        // Llave primaria compuesta
        builder.HasKey(pd => new { pd.DiagnosisId, pd.PatientId });

        builder.Property(pd => pd.DiagnosisId)
            .HasColumnName("diagnosis_id");

        builder.Property(pd => pd.PatientId)
            .HasColumnName("patient_id");

        builder.Property(pd => pd.StatusId)
            .HasColumnName("status_id")
            .IsRequired();

        builder.Property(pd => pd.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relación con el catálogo de diagnósticos
        builder.HasOne(pd => pd.Diagnosis)
            .WithMany()
            .HasForeignKey(pd => pd.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con el paciente
        builder.HasOne(pd => pd.Patient)
            .WithMany(p => p.Diagnoses)
            .HasForeignKey(pd => pd.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con el estado del diagnóstico
        builder.HasOne(pd => pd.Status)
            .WithMany()
            .HasForeignKey(pd => pd.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pd => pd.StatusId);
    }
}
