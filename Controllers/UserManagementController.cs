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

            public UserManagementController(UserManager<IdentityUser> userManager)
            {
                _userManager = userManager;
            }

            // GET: Displays all registered users
            public async Task<IActionResult> Index()
            {
                var users = await _userManager.Users.ToListAsync();
                return View(users);
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

                    await _userManager.DeleteAsync(user);
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }