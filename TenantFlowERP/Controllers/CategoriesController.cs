using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;
using TenantFlowERP.Entities;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }



        // CREATE CATEGORY
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (tenantIdClaim == null) return Unauthorized();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                TenantId = Guid.Parse(tenantIdClaim) // Required for new records
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // GET CATEGORIES
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {

            // Global Filter automatically handles TenantId and IsActive (if configured)
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // UPDATE CATEGORY
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
        {
            // The filter ensures we only find categories belonging to THIS tenant
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            category.Name = dto.Name;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully", category });
        }

        // DELETE CATEGORY
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            // Filter ensures tenant isolation
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Check if category is used by any products
            // Filter automatically applies to _context.Products too!
            var isUsed = await _context.Products
                .AnyAsync(x => x.CategoryId == id);

            if (isUsed)
            {
                category.IsActive = false; // Soft delete
                await _context.SaveChangesAsync();
                return Ok(new { message = "Category is in use and has been deactivated." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}