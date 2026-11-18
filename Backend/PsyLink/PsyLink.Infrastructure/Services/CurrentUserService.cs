
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using PsyLink.Application.Interfaces;

namespace PsyLink.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var userId =
                User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue("sub");

            return Guid.TryParse(userId, out var id)
                ? id
                : Guid.Empty;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId = User?.FindFirstValue("tenant_id");

            return Guid.TryParse(tenantId, out var id)
                ? id
                : null;
        }
    }

    public string? RoleName =>
        User?.FindFirstValue(ClaimTypes.Role)
        ?? User?.FindFirstValue("role");
}
