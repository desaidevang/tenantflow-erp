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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly TenantService _tenantService;

        public UsersController(
            AppDbContext context,
            TenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        
        // ONLY ADMIN CAN CREATE USERS
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(
            CreateUserDto dto)
        {
            // Check email already exists
            var exists = await _context.Users
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Email == dto.Email);

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Email already exists"
                });
            }

            var tenantId =
                _tenantService.GetTenantId();

            var user = new User
            {
                Id = Guid.NewGuid(),

                Name = dto.Name,

                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password),

                Role = dto.Role,

                TenantId = tenantId
            };

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully"
            });
        }

        // GET USERS OF CURRENT TENANT
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.Role
                })
                .ToListAsync();

            return Ok(users);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
    Guid id,
    UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            user.Name = dto.Name;

            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User updated successfully"
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User deactivated successfully"
            });
        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordDto dto)
        {
            var userId =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == Guid.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.CurrentPassword,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest(new
                {
                    message = "Current password incorrect"
                });
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Password changed successfully"
            });
        }
    }
}