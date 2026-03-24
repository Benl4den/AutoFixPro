using AutoFix_Pro.Data;
using AutoFix_Pro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.BookingDate)
                .ToListAsync();

            // --- CALCULATE DASHBOARD STATS ---
            ViewBag.TotalCount = appointments.Count;
            ViewBag.PendingCount = appointments.Count(a => a.Status == "Pending");
            ViewBag.CompletedCount = appointments.Count(a => a.Status == "Completed");

            return View(appointments);
        }

        // ==========================================
        // CUSTOMER SIDE: Create Booking
        // ==========================================

        public IActionResult Create(string serviceType)
        {
            // Create the model and pre-fill the ServiceType and Date
            var model = new Appointment
            {
                ServiceType = serviceType,
                BookingDate = DateTime.Now.AddDays(1) // Default to tomorrow
            };

            // Also provide the full list for the dropdown
            ViewBag.ServiceList = new SelectList(_context.ServiceTicket, "ServiceName", "ServiceName", serviceType);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,BookingDate,ServiceType,VehiclePlate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Success));
            }
            return View(appointment);
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Completed";

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}