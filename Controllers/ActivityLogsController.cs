using AutoFix_Pro.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoFix_Pro.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Restrict to SuperAdmin for security
    public class ActivityLogsController : Controller
    {
        private readonly AutoFix_ProContext _context;

        public ActivityLogsController(AutoFix_ProContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Show newest logs first
            var logs = await _context.ActivityLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
            return View(logs);
        }
    }
}