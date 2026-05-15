using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class User : ITenantEntity
    {
        public Guid Id { get; set; }

        // Keep this one (satisfies the Interface)
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; } = true;
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Admin";

        // Navigation Property
        public Tenant Tenant { get; set; } = null!;
    }
}