using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Data;
using AutoFix_Pro.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AutoFix_Pro.Controllers
{
    public class BookingController : Controller
    {
        private readonly AutoFix_ProContext _context;

        public BookingController(AutoFix_ProContext context)
        {
            _context = context;
        }

        // ==========================================
        // ADMIN SIDE: View All Appointments
        // ==========================================

        // Only users logged in as "SuperAdmin" can see this page
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            // Fetches all appointments from the database, newest first
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.BookingDate)
                .ToListAsync();

            return View(appointments);
        }

        // ==========================================
        // CUSTOMER SIDE: Create Booking
        // ==========================================

        // GET: Booking/Create
        // Catches the 'serviceType' from the "Book This Service" button
        public IActionResult Create(string serviceType)
        {
            var model = new Appointment
            {
                ServiceType = serviceType,
                BookingDate = DateTime.Now.AddDays(1) // Defaults the date to tomorrow
            };

            return View(model);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,BookingDate,ServiceType,VehiclePlate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Status defaults to "Pending" as defined in your Appointment model
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Success));
            }
            return View(appointment);
        }

        // GET: Booking/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}