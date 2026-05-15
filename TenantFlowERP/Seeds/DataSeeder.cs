using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantFlowERP.Data;
using TenantFlowERP.Entities;

namespace TenantFlowERP.Seeds
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // The specific tenant ID and email
            var targetTenantId = Guid.Parse("681383cf-7c7c-4420-9c52-b0f37c0bc85c");
            var targetTenantEmail = "1desaidevang@gmail.com";

            // Ensure tenant exists
            var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == targetTenantId);
            if (tenant == null)
            {
                tenant = new Tenant
                {
                    Id = targetTenantId,
                    CompanyName = "Desai Devang Enterprises",
                    Email = targetTenantEmail,
                    IsApproved = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Tenants.AddAsync(tenant);
                await context.SaveChangesAsync();
            }

            // Check if data already exists
            var existingProducts = await context.Products
                .Where(p => p.TenantId == targetTenantId)
                .CountAsync();

            if (existingProducts > 0)
            {
                Console.WriteLine($"Data already exists for tenant {targetTenantEmail}. Found {existingProducts} products. Skipping seed.");
                return;
            }

            Console.WriteLine($"Seeding 200-300 records for tenant: {targetTenantEmail}");

            // Seed all related data in correct order
            await SeedCategories(context, targetTenantId);
            await SeedProducts(context, targetTenantId);
            await SeedCustomers(context, targetTenantId);
            await SeedInventoryTransactions(context, targetTenantId);
            await SeedInvoicesAndItems(context, targetTenantId);

            Console.WriteLine("Seeding completed successfully!");
        }

        private static async Task SeedCategories(AppDbContext context, Guid tenantId)
        {
            var categories = new List<Category>();

            var categoryNames = new[]
            {
                "Electronics", "Clothing", "Books", "Home & Garden",
                "Sports", "Toys", "Beauty", "Automotive",
                "Music", "Office Supplies", "Pet Supplies",
                "Baby Products", "Jewelry", "Footwear", "Furniture",
                "Kitchen Appliances", "Tools", "Health & Wellness",
                "Computers", "Mobile Phones", "Cameras", "Gaming"
            };

            foreach (var categoryName in categoryNames)
            {
                categories.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName,
                    TenantId = tenantId,
                    IsActive = true
                });
            }

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {categories.Count} categories");
        }

        private static async Task SeedProducts(AppDbContext context, Guid tenantId)
        {
            var categories = await context.Categories
                .Where(c => c.TenantId == tenantId && c.IsActive)
                .ToListAsync();

            if (!categories.Any())
            {
                Console.WriteLine("❌ No categories found, skipping product seeding");
                return;
            }

            var products = new List<Product>();
            var random = new Random();

            // Seed 100-150 products
            int productCount = random.Next(100, 151);

            var productNames = new[]
            {
                "Premium Laptop", "Wireless Mouse", "Mechanical Keyboard", "USB-C Hub", "4K Monitor",
                "Cotton T-Shirt", "Slim Fit Jeans", "Winter Jacket", "Running Shoes", "Formal Shirt",
                "Python Programming", "C# Advanced", "SQL Guide", "React Masterclass", "Azure Cloud Book",
                "Coffee Maker", "Professional Blender", "Air Fryer Pro", "Robot Vacuum", "Mixer Grinder",
                "Basketball", "Soccer Ball", "Tennis Racket", "Yoga Mat", "Dumbbell Set",
                "Lego Set", "Board Game", "Action Figure", "Educational Puzzle", "RC Car",
                "Smartphone", "Wireless Headphones", "Smart Watch", "Tablet", "Power Bank",
                "Desk Lamp", "Office Chair", "Standing Desk", "Printer", "Paper Shredder"
            };

            for (int i = 0; i < productCount; i++)
            {
                var category = categories[random.Next(categories.Count)];
                var nameIndex = random.Next(productNames.Length);
                var price = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2);
                var stock = random.Next(0, 500);

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = productNames[nameIndex],
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = category.Id,
                    TenantId = tenantId
                };

                products.Add(product);
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {products.Count} products");
        }

        private static async Task SeedCustomers(AppDbContext context, Guid tenantId)
        {
            var customers = new List<Customer>();
            var random = new Random();

            // Seed 50-80 customers
            int customerCount = random.Next(50, 81);

            var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Lisa", "William", "Emma", "James", "Maria", "Charles", "Patricia", "Thomas", "Jennifer", "Daniel", "Linda", "Matthew", "Barbara" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Wilson", "Anderson", "Taylor", "Thomas", "Moore", "Jackson", "Martin", "Lee", "White", "Harris" };
            var domains = new[] { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "example.com", "company.com", "business.com" };

            for (int i = 0; i < customerCount; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var fullName = $"{firstName} {lastName}";

                customers.Add(new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = fullName,
                    Phone = $"+1 {random.Next(200, 999)}-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}{random.Next(1, 100)}@{domains[random.Next(domains.Length)]}",
                    Address = $"{random.Next(100, 9999)} {GetRandomStreet()}, {GetRandomCity()}, {GetRandomState()} {random.Next(10000, 99999)}",
                    TenantId = tenantId
                });
            }

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {customers.Count} customers");
        }

        private static async Task SeedInventoryTransactions(AppDbContext context, Guid tenantId)
        {
            var products = await context.Products
                .Where(p => p.TenantId == tenantId)
                .ToListAsync();

            if (!products.Any())
            {
                Console.WriteLine("❌ No products found, skipping inventory transactions");
                return;
            }

            var transactions = new List<InventoryTransaction>();
            var random = new Random();
            var transactionTypes = new[] { "StockIn", "Sale", "Return", "Damage" };

            // Seed 100-150 transactions
            int transactionCount = random.Next(100, 151);

            for (int i = 0; i < transactionCount; i++)
            {
                var product = products[random.Next(products.Count)];
                var type = transactionTypes[random.Next(transactionTypes.Length)];
                var quantity = random.Next(1, 50);

                transactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = quantity,
                    Type = type,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
                    TenantId = tenantId
                });
            }

            await context.InventoryTransactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {transactions.Count} inventory transactions");
        }

        private static async Task SeedInvoicesAndItems(AppDbContext context, Guid tenantId)
        {
            var customers = await context.Customers
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();

            var products = await context.Products
                .Where(p => p.TenantId == tenantId)
                .ToListAsync();

            if (!customers.Any() || !products.Any())
            {
                Console.WriteLine($"❌ Missing data for invoices - Customers: {customers.Count}, Products: {products.Count}");
                return;
            }

            var invoices = new List<Invoice>();
            var invoiceItems = new List<InvoiceItem>();
            var random = new Random();

            // Seed 30-50 invoices
            int invoiceCount = random.Next(30, 51);

            for (int i = 0; i < invoiceCount; i++)
            {
                var customer = customers[random.Next(customers.Count)];
                var invoiceDate = DateTime.UtcNow.AddDays(-random.Next(0, 60));

                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = $"INV-{invoiceDate:yyyyMMdd}-{i + 1:D4}",
                    CustomerId = customer.Id,
                    CreatedAt = invoiceDate,
                    TenantId = tenantId,
                    TotalAmount = 0 // Will calculate later
                };

                invoices.Add(invoice);

                // Each invoice has 2-8 items
                int itemCount = random.Next(2, 9);
                decimal invoiceTotal = 0;

                for (int j = 0; j < itemCount; j++)
                {
                    var product = products[random.Next(products.Count)];
                    var quantity = random.Next(1, 6);
                    var price = product.Price;
                    var total = price * quantity;
                    invoiceTotal += total;

                    invoiceItems.Add(new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = price,     // Using Price as per your entity
                        Total = total      // Using Total as per your entity
                    });
                }

                invoice.TotalAmount = invoiceTotal;
            }

            // Save invoices first
            await context.Invoices.AddRangeAsync(invoices);
            await context.SaveChangesAsync();

            // Then save invoice items
            await context.InvoiceItems.AddRangeAsync(invoiceItems);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {invoices.Count} invoices with {invoiceItems.Count} items");
            Console.WriteLine($"📊 Summary: Categories, Products, Customers, Transactions, Invoices all seeded!");
        }

        // Helper methods with static Random to avoid seed issues
        private static readonly Random _random = new Random();

        private static string GetRandomStreet()
        {
            var streets = new[] { "Main St", "Oak Ave", "Maple Dr", "Cedar Ln", "Pine Rd", "Elm Blvd", "Washington St", "Park Ave", "Lake Shore Dr", "Broadway" };
            return streets[_random.Next(streets.Length)];
        }

        private static string GetRandomCity()
        {
            var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "Austin" };
            return cities[_random.Next(cities.Length)];
        }

        private static string GetRandomState()
        {
            var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "FL", "OH", "GA", "NC" };
            return states[_random.Next(states.Length)];
        }
    }
}