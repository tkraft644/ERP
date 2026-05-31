using ErpSystem.Application.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ErpSystem.Infrastructure.Security;

public sealed class DefaultCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public DefaultCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var rawValue = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

            return int.TryParse(rawValue, out var userId) ? userId : null;
        }
    }
}
