using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Data;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // DASHBOARD STATS
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // Total Products
            var totalProducts =
                await _context.Products.CountAsync();

            // Total Customers
            var totalCustomers =
                await _context.Customers.CountAsync();

            // Total Invoices
            var totalInvoices =
                await _context.Invoices.CountAsync();

            // Total Revenue
            var totalRevenue =
                await _context.Invoices
                    .SumAsync(x => x.TotalAmount);

            // Low Stock Products
            var lowStockProducts =
                await _context.Products
                    .CountAsync(x =>
                        x.StockQuantity <= 5);

            return Ok(new
            {
                totalProducts,
                totalCustomers,
                totalInvoices,
                totalRevenue,
                lowStockProducts
            });
        }
        [HttpGet("recent-sales")]
        public async Task<IActionResult> RecentSales()
        {
            var sales = await _context.Invoices
                .Include(x => x.Customer)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x => new
                {
                    x.InvoiceNumber,
                    Customer = x.Customer.Name,
                    x.TotalAmount,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(sales);
        }
        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStockProducts()
        {
            var products = await _context.Products
                .Where(x => x.StockQuantity <= 5)
                .Select(x => new
                {
                    x.Name,
                    x.StockQuantity,
                    x.Price
                })
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts()
        {
            var topProducts = await _context.InvoiceItems
                .Include(x => x.Product)
                .GroupBy(x => x.Product.Name)
                .Select(g => new
                {
                    Product = g.Key,

                    TotalSold = g.Sum(x => x.Quantity),

                    Revenue = g.Sum(x => x.Total)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToListAsync();

            return Ok(topProducts);
        }
        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> MonthlyRevenue()
        {
            var revenue = await _context.Invoices
                .GroupBy(x => new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
                .Select(g => new
                {
                    Month =
                        g.Key.Month + "/" + g.Key.Year,

                    Revenue =
                        g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Ok(revenue);
        }
        [HttpGet("recent-transactions")]
        public async Task<IActionResult> RecentTransactions()
        {
            var transactions =
                await _context.InventoryTransactions
                    .Include(x => x.Product)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .Select(x => new
                    {
                        Product = x.Product.Name,
                        x.Type,
                        x.Quantity,
                        x.CreatedAt
                    })
                    .ToListAsync();

            return Ok(transactions);
        }
    }
}