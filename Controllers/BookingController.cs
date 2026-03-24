using AutoFix_Pro.Data;
using AutoFix_Pro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        // ADMIN PANEL: Professional Dashboard
        // Visible to BOTH Admin and SuperAdmin
        // ==========================================
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Dashboard()
        {
            var appointments = await _context.Appointments.ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalAppointments = appointments.Count,
                PendingTasks = appointments.Count(a => a.Status == "Pending" || a.BookingDate > DateTime.Now),
                TotalRevenue = appointments.Count(a => a.Status == "Completed") * 2500,

                ChartLabels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                ChartData = new List<int> { 4, 7, 10, 5, 12, 8, 3 }
            };

            return View(viewModel);
        }

        // ==========================================
        // ADMIN SIDE: View All Appointments (Table View)
        // RESTRICTED: Only SuperAdmin can see this list
        // ==========================================
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.BookingDate)
                .ToListAsync();

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
            var model = new Appointment
            {
                ServiceType = serviceType,
                BookingDate = DateTime.Now.AddDays(1)
            };

            ViewBag.ServiceList = new SelectList(_context.ServiceTicket, "ServiceName", "ServiceName", serviceType);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,BookingDate,ServiceType,VehiclePlate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Status = "Pending";
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

        // ==========================================
        // ADMIN ACTIONS: Update Status
        // RESTRICTED: Only SuperAdmin can complete a booking
        // ==========================================
        [Authorize(Roles = "SuperAdmin")]
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