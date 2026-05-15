using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;
using TenantFlowERP.Entities;
using TenantFlowERP.Interfaces;
using TenantFlowERP.Services;

namespace TenantFlowERP.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly TenantService _tenantService;

        public InvoiceService(
            AppDbContext context,
            TenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // CREATE INVOICE
        public async Task<object> CreateInvoice(CreateInvoiceDto dto)
        {
            var tenantId = _tenantService.GetTenantId();

            if (tenantId == Guid.Empty)
            {
                return new
                {
                    message = "Unauthorized"
                };
            }

            // Check customer exists
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId);

            if (customer == null)
            {
                return new
                {
                    message = "Customer not found"
                };
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),

                    InvoiceNumber =
                        $"INV-{DateTime.UtcNow.Ticks}",

                    CustomerId = dto.CustomerId,

                    TenantId = tenantId,

                    TotalAmount = 0
                };

                await _context.Invoices.AddAsync(invoice);

                decimal grandTotal = 0;

                foreach (var item in dto.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(x =>
                            x.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new Exception(
                            "Product not found");
                    }

                    // Check stock
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new Exception(
                            $"Insufficient stock for {product.Name}");
                    }

                    // Reduce stock
                    product.StockQuantity -= item.Quantity;

                    // Calculate total
                    var itemTotal =
                        product.Price * item.Quantity;

                    grandTotal += itemTotal;

                    // Create invoice item
                    var invoiceItem = new InvoiceItem
                    {
                        Id = Guid.NewGuid(),

                        InvoiceId = invoice.Id,

                        ProductId = product.Id,

                        Quantity = item.Quantity,

                        Price = product.Price,

                        Total = itemTotal
                    };
                    await _context.InvoiceItems
                                            .AddAsync(invoiceItem);

                    // Create inventory transaction
                    var inventoryTransaction =
                        new InventoryTransaction
                        {
                            Id = Guid.NewGuid(),

                            ProductId = product.Id,

                            Quantity = item.Quantity,

                            Type = "Sale",

                            TenantId = tenantId
                        };

                    await _context.InventoryTransactions
                        .AddAsync(inventoryTransaction);
                }

                invoice.TotalAmount = grandTotal;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new
                {
                    message = "Invoice created successfully",
                    invoice.Id,
                    invoice.InvoiceNumber,
                    invoice.TotalAmount
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new
                {
                    message = ex.Message
                };
            }
        }
        // GET ALL INVOICES
        public async Task<object> GetInvoices(
    int page,
    int pageSize,
    string? search)
        {
            pageSize = Math.Min(pageSize, 50);

            var query = _context.Invoices
                .Include(x => x.Customer)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.InvoiceNumber.ToLower().Contains(search)

                    ||

                    x.Customer.Name.ToLower().Contains(search));
            }

            var totalInvoices =
                await query.CountAsync();

            var invoices = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.InvoiceNumber,
                    Customer = x.Customer.Name,
                    x.TotalAmount,
                    x.CreatedAt
                })
                .ToListAsync();

            return new
            {
                page,
                pageSize,
                totalInvoices,

                totalPages =
                    (int)Math.Ceiling(
                        totalInvoices / (double)pageSize),

                search,

                data = invoices
            };
        }
        // GET INVOICE BY ID
        public async Task<object> GetInvoiceById(Guid id)
        {
            var invoice = await _context.Invoices
                .Include(x => x.Customer)
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.InvoiceNumber,
                    Customer = x.Customer.Name,
                    x.TotalAmount,
                    x.CreatedAt,

                    Items = x.Items.Select(i => new
                    {
                        Product = i.Product.Name,
                        i.Quantity,
                        i.Price,
                        i.Total
                    })
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return new
                {
                    message = "Invoice not found"
                };
            }

            return invoice;
        }
    }
}