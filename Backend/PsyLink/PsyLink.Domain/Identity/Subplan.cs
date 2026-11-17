namespace PsyLink.Domain.Identity;

public class Subplan
{
    public Guid SubplanId { get; set; }
    public Guid PlanId { get; set; }

    /// <summary>VARCHAR(20).</summary>
    public string Name { get; set; } = string.Empty;

    public short CantHours { get; set; }
    public decimal Price { get; set; }

    public Plan Plan { get; set; } = null!;
}
