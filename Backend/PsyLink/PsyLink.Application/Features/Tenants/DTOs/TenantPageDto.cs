namespace PsyLink.Application.Features.Tenants.DTOs;

public class TenantPageDto
{
    public List<TenantDto> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
