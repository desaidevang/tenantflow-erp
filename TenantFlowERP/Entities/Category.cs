using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class Category : ITenantEntity
    {
        public Guid Id { get; set; }

        // REMOVE THE DUPLICATE: Keep only this one
        public Guid TenantId { get; set; }

        public string Name { get; set; } = string.Empty;

        // Soft Delete
        public bool IsActive { get; set; } = true;

        // Navigation Property
        public Tenant Tenant { get; set; } = null!;

        // One Category -> Many Products
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}