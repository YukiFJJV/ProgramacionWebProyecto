
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class SubplanConfiguration : IEntityTypeConfiguration<Subplan>
{
    public void Configure(EntityTypeBuilder<Subplan> builder)
    {
        builder.ToTable("subplans");

        builder.HasKey(s => s.SubplanId);

        builder.Property(s => s.SubplanId)
            .HasColumnName("subplan_id");

        builder.Property(s => s.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CantHours)
            .HasColumnName("cant_hours")
            .IsRequired();

        builder.Property(s => s.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(s => s.Plan)
            .WithMany()
            .HasForeignKey(s => s.PlanId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
