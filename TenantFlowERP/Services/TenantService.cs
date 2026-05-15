using System.Security.Claims;

namespace TenantFlowERP.Services
{
    public class TenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetTenantId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("TenantId")?.Value;
            return string.IsNullOrEmpty(claim) ? Guid.Empty : Guid.Parse(claim);
        }
    }
}