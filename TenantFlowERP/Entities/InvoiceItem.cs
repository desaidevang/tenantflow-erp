namespace TenantFlowERP.Entities
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        // Invoice Relationship
        public Guid InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        // Product Relationship
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        // Sale Details
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
    }
}