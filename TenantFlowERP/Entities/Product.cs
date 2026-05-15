using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class Product : ITenantEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        // Category Relationship
        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        // Tenant Relationship (Implementing ITenantEntity)
        // Keep ONLY this one instance of TenantId
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }
}