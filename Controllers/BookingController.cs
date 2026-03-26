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
        // ==========================================
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Dashboard()
        {
            var appointments = await _context.Appointments.ToListAsync();
            var services = await _context.ServiceTicket.ToListAsync();

            // 1. Calculate Actual Revenue (Completed)
            decimal accurateRevenue = appointments
                .Where(a => !string.IsNullOrEmpty(a.Status) && a.Status.Trim().Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .Sum(a => {
                    var matchedService = services.FirstOrDefault(s =>
                        a.ServiceType != null && (
                        a.ServiceType.Contains(s.ServiceName, StringComparison.OrdinalIgnoreCase) ||
                        s.ServiceName.Contains(a.ServiceType, StringComparison.OrdinalIgnoreCase)));
                    return matchedService?.Price ?? 0;
                });

            // 2. Calculate Projected Revenue (Pending)
            decimal projectedRevenue = appointments
                .Where(a => !string.IsNullOrEmpty(a.Status) && a.Status.Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .Sum(a => {
                    var matchedService = services.FirstOrDefault(s =>
                        a.ServiceType != null && (
                        a.ServiceType.Contains(s.ServiceName, StringComparison.OrdinalIgnoreCase) ||
                        s.ServiceName.Contains(a.ServiceType, StringComparison.OrdinalIgnoreCase)));
                    return matchedService?.Price ?? 0;
                });

            var viewModel = new AdminDashboardViewModel
            {
                TotalAppointments = appointments.Count,
                PendingTasks = appointments.Count(a => a.Status == "Pending"),
                TotalRevenue = accurateRevenue,
                // FIXED: Now correctly assigning the calculated value to the ViewModel property
                ProjectedRevenue = projectedRevenue,
                ChartLabels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                ChartData = new List<int> { 4, 7, 10, 5, 12, 8, 3 }
            };

            return View(viewModel);
        }

        // ==========================================
        // ADMIN SIDE: View All Appointments (Table View)
        // ==========================================
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                // 1. Sort by Status: "Pending" gets priority (0), everything else comes after (1)
                .OrderBy(a => a.Status == "Pending" ? 0 : 1)
                // 2. Secondary sort: Keep the most recent bookings at the top within those groups
                .ThenByDescending(a => a.BookingDate)
                .ToListAsync();

            var services = await _context.ServiceTicket.ToListAsync();
            ViewBag.Services = services;

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
        // ADMIN ACTIONS: Update Status & Log Activity
        // ==========================================
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            // 1. Update the record status
            appointment.Status = "Completed";

            // 2. Create the audit log entry
            var log = new ActivityLog
            {
                UserEmail = User.Identity.Name ?? "System",
                Action = "Status Update",
                Details = $"Marked Appointment ID {id} for {appointment.CustomerName} as Completed",
                Timestamp = DateTime.Now
            };

            // 3. Add both changes to the context tracking
            _context.ActivityLogs.Add(log);
            _context.Update(appointment);

            // 4. Save both changes to the database in one single transaction
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}