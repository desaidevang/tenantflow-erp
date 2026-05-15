using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Data;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch(
            string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new
                {
                    message = "Search query required"
                });
            }

            query = query.ToLower();

            // PRODUCTS
            var products = await _context.Products
                .Include(x => x.Category)
                .Where(x =>
                    x.Name.ToLower().Contains(query))
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    Type = "Product"
                })
                .ToListAsync();

            // CUSTOMERS
            var customers = await _context.Customers
                .Where(x =>
                    x.Name.ToLower().Contains(query) ||
                    x.Email.ToLower().Contains(query))
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    Type = "Customer"
                })
                .ToListAsync();

            // INVOICES
            var invoices = await _context.Invoices
                .Include(x => x.Customer)
                .Where(x =>
                    x.InvoiceNumber.ToLower()
                        .Contains(query))
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    x.InvoiceNumber,
                    Customer = x.Customer.Name,
                    x.TotalAmount,
                    Type = "Invoice"
                })
                .ToListAsync();

            return Ok(new
            {
                products,
                customers,
                invoices
            });
        }
    }
}