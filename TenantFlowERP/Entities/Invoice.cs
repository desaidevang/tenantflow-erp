using TenantFlowERP.Interfaces;

namespace TenantFlowERP.Entities
{
    public class Invoice : ITenantEntity
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }
            = string.Empty;

        // Customer Relationship
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        // Total Bill
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Tenant Isolation
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        // One Invoice -> Many Items
        public ICollection<InvoiceItem> Items { get; set; }
            = new List<InvoiceItem>();
    }
}