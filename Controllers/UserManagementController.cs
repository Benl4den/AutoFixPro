using AutoFix_Pro.Data;
using AutoFix_Pro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoFix_Pro.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Only the big boss gets in here
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AutoFix_ProContext _context;

        public UserManagementController(UserManager<IdentityUser> userManager, AutoFix_ProContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Displays all registered users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // POST: Add a new Admin
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email and Password are required.");
            }

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign the "Admin" role to the new user
                await _userManager.AddToRoleAsync(user, "Admin");

                // --- LOGGING START ---
                var log = new ActivityLog
                {
                    UserEmail = User.Identity.Name ?? "System",
                    Action = "User Created",
                    Details = $"SuperAdmin created a new admin account: {email}",
                    Timestamp = DateTime.Now
                };
                _context.ActivityLogs.Add(log);
                await _context.SaveChangesAsync();
                // --- LOGGING END ---

                return RedirectToAction(nameof(Index));
            }

            return BadRequest("Failed to create admin. Password might be too simple or email exists.");
        }

        // POST: Delete a user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Prevent the SuperAdmin from deleting themselves!
                if (user.Email == User.Identity.Name)
                {
                    return BadRequest("You cannot delete your own account.");
                }

                // --- LOGGING START ---
                var log = new ActivityLog
                {
                    UserEmail = User.Identity.Name ?? "System",
                    Action = "User Deleted",
                    Details = $"SuperAdmin deleted admin account: {user.Email}",
                    Timestamp = DateTime.Now
                };
                _context.ActivityLogs.Add(log);
                // --- LOGGING END ---

                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}