using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class InventoryTransaction : ITenantEntity
    {
        public Guid Id { get; set; }

        // Keep this one (satisfies the ITenantEntity interface)
        public Guid TenantId { get; set; }

        // Product Relationship
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        // Quantity Changed
        public int Quantity { get; set; }

        // StockIn, Sale, Return, Damage
        public string Type { get; set; } = string.Empty;

        // Date Time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Tenant Tenant { get; set; } = null!;
    }
}