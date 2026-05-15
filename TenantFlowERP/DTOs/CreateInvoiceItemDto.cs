namespace TenantFlowERP.DTOs
{
    public class CreateInvoiceItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}