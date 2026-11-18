namespace PsyLink.Infrastructure.Persistence.Entities.Clinical;

public sealed class AppointmentSessionNotes
{
    public Guid AppointmentId { get; set; }
    public Guid TenantId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
