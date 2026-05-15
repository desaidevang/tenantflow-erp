using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Entities;
using TenantFlowERP.Interfaces;
using TenantFlowERP.Services;

namespace TenantFlowERP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly TenantService _tenantService;

        public AppDbContext(DbContextOptions<AppDbContext> options, TenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }
        // Dynamic TenantId Per Request
        public Guid CurrentTenantId =>
            _tenantService.GetTenantId();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(x =>
                    x.TenantId == CurrentTenantId);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(x =>
                    x.TenantId == CurrentTenantId);

            modelBuilder.Entity<Customer>()
                .HasQueryFilter(x =>
                    x.TenantId == CurrentTenantId);

            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(x =>
                    x.TenantId == CurrentTenantId);

            modelBuilder.Entity<InventoryTransaction>()
                .HasQueryFilter(x =>
                    x.TenantId == CurrentTenantId);

            modelBuilder.Entity<User>()
     .HasQueryFilter(x =>
         x.TenantId == CurrentTenantId &&
         x.IsActive);
        }

        // Helper to build the filter expression: x => x.TenantId == currentTenantId
        private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(
            Type type, Guid tenantId)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(type, "x");
            var property = System.Linq.Expressions.Expression.Property(parameter, "TenantId");
            var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(tenantId));
            return System.Linq.Expressions.Expression.Lambda(condition, parameter);
        }
    }
}