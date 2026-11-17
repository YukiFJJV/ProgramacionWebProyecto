
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");

        builder.HasKey(p => p.PlanId);

        builder.Property(p => p.PlanId)
            .HasColumnName("plan_id");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired();
    }
}
