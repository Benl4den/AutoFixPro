using AutoFix_Pro.Data;
using AutoFix_Pro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// CHANGED: Switched from UseSqlServer to UseSqlite
builder.Services.AddDbContext<AutoFix_ProContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=AutoFixPro.db"));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AutoFix_ProContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- SEEDING LOGIC START ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Original SeedData for your Car/Service models
    SeedData.Initialize(services);

    // Identity Seeding for Roles and SuperAdmin
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Create Roles
    string[] roleNames = { "SuperAdmin", "Admin" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Create the Default SuperAdmin User
    string adminEmail = "superadmin@autofix.pro";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        // This creates the user with the password "Super123!"
        var result = await userManager.CreateAsync(newAdmin, "Super123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "SuperAdmin");
        }
    }
}
// --- SEEDING LOGIC END ---

app.Run();