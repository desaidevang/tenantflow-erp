using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;
using TenantFlowERP.Entities;
using TenantFlowERP.Services;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TenantService _tenantService;

        public CustomersController(
            AppDbContext context,
            TenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // CREATE CUSTOMER
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto dto)
        {
            var tenantId = _tenantService.GetTenantId();

            if (tenantId == Guid.Empty)
            {
                return Unauthorized();
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),

                Name = dto.Name,

                Phone = dto.Phone,

                Email = dto.Email,

                Address = dto.Address,

                TenantId = tenantId
            };

            await _context.Customers.AddAsync(customer);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                customer.Id,
                customer.Name,
                customer.Phone,
                customer.Email,
                customer.Address
            });
        }

        // GET ALL CUSTOMERS
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetCustomers(
     int page = 1,
     int pageSize = 10,
     string? search = null)
        {
            // Prevent huge requests
            pageSize = Math.Min(pageSize, 50);

            // Start query
            var query = _context.Customers
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||

                    x.Email.ToLower().Contains(search) ||

                    x.Phone.Contains(search));
            }

            // Count AFTER filtering
            var totalCustomers =
                await query.CountAsync();

            // Pagination
            var customers = await query
                .OrderByDescending(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalCustomers,

                totalPages =
                    (int)Math.Ceiling(
                        totalCustomers / (double)pageSize),

                search,

                data = customers
            });
        }

        // GET CUSTOMER BY ID
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            return Ok(customer);
        }

        // UPDATE CUSTOMER
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            Guid id,
            UpdateCustomerDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Customer updated successfully"
            });
        }

        // DELETE CUSTOMER
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Customer deleted successfully"
            });
        }
    }
}