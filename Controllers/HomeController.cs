using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Data;
using AutoFix_Pro.Models;

namespace AutoFix_Pro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AutoFix_ProContext _context;

        // Constructor handles both Logging and Database access
        public HomeController(ILogger<HomeController> logger, AutoFix_ProContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Where(s => s.IsActive) - only shows services that are turned on
            // 2. OrderByDescending - puts the newest repairs at the top
            // 3. Take(3) - keeps the homepage clean by only showing the "Top 3"
            var services = await _context.ServiceTicket
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.Id)
                .Take(3)
                .ToListAsync();

            return View(services);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}