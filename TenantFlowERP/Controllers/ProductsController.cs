using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;
using TenantFlowERP.Services;
using TenantFlowERP.Entities;


namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly TenantService _tenantService;
        public ProductsController(AppDbContext context, TenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // CREATE PRODUCT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (tenantIdClaim == null) return Unauthorized();
            var tenantId = Guid.Parse(tenantIdClaim);

            // The Global Filter ensures we only find a category if it belongs to THIS tenant
            var categoryExists = await _context.Categories
                .AnyAsync(x => x.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new { message = "Invalid category" });
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                TenantId = tenantId // Required for the new database record
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // GET ALL PRODUCTS
       [Authorize(Roles = "Admin,Staff")]
[HttpGet]
public async Task<IActionResult> GetProducts(
    int page = 1,
    int pageSize = 10,
    string? search = null)
{
    // Prevent huge requests
    pageSize = Math.Min(pageSize, 50);

    // Start query
    var query = _context.Products
        .Include(x => x.Category)
        .AsQueryable();

    // SEARCH
    if (!string.IsNullOrWhiteSpace(search))
    {
        search = search.ToLower();

        query = query.Where(x =>
            x.Name.ToLower().Contains(search.ToLower()));
    }

    // Total count AFTER filtering
    var totalProducts =
        await query.CountAsync();

    // Pagination
    var products = await query
        .OrderByDescending(x => x.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new
        {
            x.Id,
            x.Name,
            x.Price,
            x.StockQuantity,
            Category = x.Category.Name
        })
        .ToListAsync();

    return Ok(new
    {
        page,
        pageSize,
        totalProducts,

        totalPages =
            (int)Math.Ceiling(
                totalProducts / (double)pageSize),

        search,

        data = products
    });
}

        // GET PRODUCT BY ID
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    x.StockQuantity,
                    x.CategoryId,
                    Category = x.Category.Name
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }

        // UPDATE PRODUCT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Verify the new category belongs to the tenant
            var categoryExists = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Invalid category" });
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Product updated successfully" });
        }

        // DELETE PRODUCT
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }

        // STOCK IN
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("stock-in")]
        public async Task<IActionResult> StockIn(StockInDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            var tenantId = Guid.Parse(User.FindFirst("TenantId")!.Value);
            product.StockQuantity += dto.Quantity;

            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Type = "StockIn",
                TenantId = tenantId
            };

            await _context.InventoryTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stock added successfully", currentStock = product.StockQuantity });
        }

        // SELL PRODUCT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("sell")]
        public async Task<IActionResult> SellProduct(SellProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            if (product.StockQuantity < dto.Quantity)
            {
                return BadRequest(new { message = "Insufficient stock" });
            }

            var tenantId = Guid.Parse(User.FindFirst("TenantId")!.Value);
            product.StockQuantity -= dto.Quantity;

            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Type = "Sale",
                TenantId = tenantId
            };

            await _context.InventoryTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product sold successfully", remainingStock = product.StockQuantity });
        }

        // PRODUCT TRANSACTION HISTORY
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetProductTransactions(Guid id)
        {
            var transactions = await _context.InventoryTransactions
                .Where(x => x.ProductId == id)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Type,
                    x.Quantity,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(transactions);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("damage")]
        public async Task<IActionResult> DamageStock(
    DamageStockDto dto)
        {
            var tenantId =
                _tenantService.GetTenantId();

            var product = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.ProductId);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            // Check stock
            if (product.StockQuantity < dto.Quantity)
            {
                return BadRequest(new
                {
                    message = "Insufficient stock"
                });
            }

            // Reduce stock
            product.StockQuantity -= dto.Quantity;

            // Save transaction
            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),

                ProductId = product.Id,

                Quantity = dto.Quantity,

                Type = "Damage",

                TenantId = tenantId
            };

            await _context.InventoryTransactions
                .AddAsync(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Damaged stock removed",
                remainingStock = product.StockQuantity
            });
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("return")]
        public async Task<IActionResult> ReturnStock(
    ReturnStockDto dto)
        {
            var tenantId =
                _tenantService.GetTenantId();

            var product = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.ProductId);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            // Increase stock
            product.StockQuantity += dto.Quantity;

            // Save transaction
            var transaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),

                ProductId = product.Id,

                Quantity = dto.Quantity,

                Type = "Return",

                TenantId = tenantId
            };

            await _context.InventoryTransactions
                .AddAsync(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Returned stock added",
                currentStock = product.StockQuantity
            });
        }
    }
}