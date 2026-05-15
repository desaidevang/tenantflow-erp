using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;
using TenantFlowERP.Entities;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // REGISTER: Create a new Tenant and an Admin User
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // 1. Create the Tenant first
            var requireApproval =
         _configuration.GetValue<bool>(
             "PlatformSettings:RequireTenantApproval");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),

                CompanyName = dto.CompanyName,

                CreatedAt = DateTime.UtcNow,
            };

            await _context.Tenants.AddAsync(tenant);

            // 2. Create the User linked to that Tenant
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Admin",
                TenantId = tenant.Id
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Company and Admin registered successfully" });
        }

        // LOGIN: Authenticate and return JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // CRITICAL: We use IgnoreQueryFilters() because the user isn't 
            // authenticated yet, so the Global Filter has no TenantId to use.
            var user = await _context.Users
          .Include(x => x.Tenant)
          .IgnoreQueryFilters()
          .FirstOrDefaultAsync(x =>
              x.Email == dto.Email);



            if (user == null)
            {
                return BadRequest(new { message = "User or Organization not found" });
            }
            if (!user.Tenant.IsApproved)
            {
                return BadRequest(new
                {
                    message =
                        "Your company account is pending approval"
                });
            }
            if (!user.Tenant.IsActive)
            {
                return BadRequest(new
                {
                    message =
                        "Your company account is deactivated"
                });
            }

            bool isPasswordValid =
        BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest(new
                {
                    message = "Invalid password"
                });
            }

            // Generate JWT Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                // This is the most important claim for our Multi-Tenancy
                new Claim("TenantId", user.TenantId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwt });
        }

    }
}