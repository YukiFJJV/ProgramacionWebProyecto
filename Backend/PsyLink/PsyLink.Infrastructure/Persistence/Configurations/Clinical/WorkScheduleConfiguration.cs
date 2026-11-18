
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("work_schedules", table =>
        {
            table.HasCheckConstraint(
                "ck_work_schedules_valid_time",
                "ends_at > starts_at");
        });

        builder.HasKey(w => w.ScheduleId);

        builder.Property(w => w.ScheduleId)
            .HasColumnName("schedule_id");

        builder.Property(w => w.PsychologistId)
            .HasColumnName("psychologist_id")
            .IsRequired();

        builder.Property(w => w.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(w => w.WeekDay)
            .HasColumnName("week_day")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(w => w.StartsAt)
            .HasColumnName("starts_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(w => w.EndsAt)
            .HasColumnName("ends_at")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.HasOne(w => w.Psychologist)
            .WithMany(p => p.Schedules)
            .HasForeignKey(w => w.PsychologistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(w => w.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => new
        {
            w.PsychologistId,
            w.WeekDay,
            w.StartsAt,
            w.EndsAt
        });

        builder.HasIndex(w => w.TenantId);
    }
}
