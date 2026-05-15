namespace TenantFlowERP.DTOs
{
    public class CreateInvoiceDto
    {
        public Guid CustomerId { get; set; }

        public List<CreateInvoiceItemDto> Items { get; set; }
            = new();
    }
}