namespace PsyLink.Domain.Identity;

public class Subscription
{
    public Guid SubscriptionId { get; set; }
    public Guid SubplanId { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }

    public Subplan Subplan { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
