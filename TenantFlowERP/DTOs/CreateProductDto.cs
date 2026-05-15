namespace TenantFlowERP.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public Guid CategoryId { get; set; }
    }
}