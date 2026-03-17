using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFix_Pro.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AutoFix_ProContext(
                serviceProvider.GetRequiredService<DbContextOptions<AutoFix_ProContext>>()))
            {
                // CRITICAL FIX: This ensures the database and tables are created 
                // before the app tries to check for existing data.
                context.Database.EnsureCreated();

                // Look for any existing services.
                if (context.ServiceTicket.Any())
                {
                    return;   // DB has been seeded already
                }

                context.ServiceTicket.AddRange(
                    new ServiceTicket
                    {
                        ServiceName = "Full Synthetic Oil Change",
                        Category = "Maintenance",
                        Description = "Premium synthetic oil change including filter replacement and 15-point inspection.",
                        Price = 2500,
                        ImageUrl = "https://images.unsplash.com/photo-1486006396113-ad7b32767211?auto=format&fit=crop&q=80&w=500",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    },
                    new ServiceTicket
                    {
                        ServiceName = "Brake Pad Replacement",
                        Category = "Repair",
                        Description = "Front and rear brake pad replacement with high-quality ceramic pads.",
                        Price = 4500,
                        ImageUrl = "https://images.unsplash.com/photo-1486262715619-67b85e0b08d3?auto=format&fit=crop&q=80&w=500",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    },
                    new ServiceTicket
                    {
                        ServiceName = "Computerized Engine Scan",
                        Category = "Diagnostics",
                        Description = "Full system scan to identify error codes and performance issues.",
                        Price = 1200,
                        ImageUrl = "https://images.unsplash.com/photo-1599256621730-535171e28e50?auto=format&fit=crop&q=80&w=500",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    }
                );
                context.SaveChanges();
            }
        }
    }
}