using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TenantFlowERP.Data;
using TenantFlowERP.DTOs;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/platform")]
    public class PlatformController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly AppDbContext _context;

        public PlatformController(
            IConfiguration configuration,
            AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // MASTER ADMIN LOGIN
        [HttpPost("login")]
        public IActionResult Login(
            PlatformLoginDto dto)
        {
            var email =
                _configuration["MasterAdmin:Email"];

            var password =
                _configuration["MasterAdmin:Password"];

            if (dto.Email != email ||
                dto.Password != password)
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid master admin credentials"
                });
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Email,
                    dto.Email),

                new Claim(
                    ClaimTypes.Role,
                    "MasterAdmin")
            };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]!));

            var creds =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer:
                        _configuration["Jwt:Issuer"],

                    audience:
                        _configuration["Jwt:Audience"],

                    claims: claims,

                    expires:
                        DateTime.UtcNow.AddDays(1),

                    signingCredentials: creds
                );

            var jwt =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return Ok(new
            {
                token = jwt
            });
        }

        // GET ALL TENANTS
        [Authorize(Roles = "MasterAdmin")]
        [HttpGet("tenants")]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _context.Tenants
                .IgnoreQueryFilters()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.CompanyName,
                    x.IsApproved,
                    x.IsActive,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(tenants);
        }

        // APPROVE TENANT
        [Authorize(Roles = "MasterAdmin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveTenant(
            Guid id)
        {
            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
            {
                return NotFound(new
                {
                    message = "Tenant not found"
                });
            }

            tenant.IsApproved = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Tenant approved successfully"
            });
        }

        // DEACTIVATE TENANT
        [Authorize(Roles = "MasterAdmin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateTenant(
            Guid id)
        {
            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
            {
                return NotFound(new
                {
                    message = "Tenant not found"
                });
            }

            tenant.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Tenant deactivated successfully"
            });
        }
    }
}