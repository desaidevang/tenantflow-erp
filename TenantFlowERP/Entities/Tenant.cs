namespace TenantFlowERP.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }

        public bool IsApproved { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<InventoryTransaction>
    InventoryTransactions
        { get; set; }
    = new List<InventoryTransaction>();
        public ICollection<Category> Categories { get; set; }
    = new List<Category>();
    }
}