namespace PsyLink.Application.Features.Tenants.DTOs;

public class TenantDto
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? PhoneId { get; set; }
    public string? Rtn { get; set; }
    public bool Active { get; set; }
}
