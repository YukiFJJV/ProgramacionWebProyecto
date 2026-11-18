using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence.Entities.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public sealed class AppointmentSessionNotesConfiguration
    : IEntityTypeConfiguration<AppointmentSessionNotes>
{
    public void Configure(EntityTypeBuilder<AppointmentSessionNotes> builder)
    {
        builder.ToTable("appointment_session_notes");

        builder.HasKey(notes => notes.AppointmentId);

        builder.Property(notes => notes.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(notes => notes.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(notes => notes.Notes)
            .HasColumnName("notes")
            .IsRequired();

        builder.Property(notes => notes.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne<Appointment>()
            .WithOne()
            .HasForeignKey<AppointmentSessionNotes>(notes => notes.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(notes => notes.TenantId);
    }
}
