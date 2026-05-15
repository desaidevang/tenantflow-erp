using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class Customer : ITenantEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        // Tenant Isolation
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}