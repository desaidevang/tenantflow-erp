using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TenantFlowERP.Data;
using TenantFlowERP.Seeds;
using TenantFlowERP.Interfaces;
using QuestPDF.Infrastructure;
using TenantFlowERP.Middleware;
using TenantFlowERP.Services;
QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TenantService>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<InvoicePdfService>();
// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Your React app URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Then after app building

// JWT Authentication
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

var app = builder.Build();
app.UseCors("AllowReactApp");
// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await DataSeeder.SeedDataAsync(scope.ServiceProvider);
}

// HTTPS
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
// Authentication
app.UseAuthentication();

app.UseAuthorization();

// Controllers
app.MapControllers();

// Test Route
app.MapGet("/",
    () => "TenantFlow ERP API Running...");

app.Run();