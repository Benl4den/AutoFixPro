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
    public class ServiceTicketsController : Controller
    {
        private readonly AutoFix_ProContext _context;

        public ServiceTicketsController(AutoFix_ProContext context)
        {
            _context = context;
        }

        // GET: ServiceTickets
        public async Task<IActionResult> Index(string searchString, string category)
        {
            var services = from s in _context.ServiceTicket
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                services = services.Where(s => s.ServiceName.ToLower().Contains(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                services = services.Where(x => x.Category == category);
            }

            return View(await services.ToListAsync());
        }

        // GET: ServiceTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceTicket = await _context.ServiceTicket
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceTicket == null) return NotFound();

            return View(serviceTicket);
        }

        // GET: ServiceTickets/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceTickets/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceName,Description,Price,ImageUrl,Category,CreatedAt,IsActive")] ServiceTicket serviceTicket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceTicket);

                // --- LOGGING START ---
                var log = new ActivityLog
                {
                    UserEmail = User.Identity.Name ?? "System",
                    Action = "Service Created",
                    Details = $"Added new service: {serviceTicket.ServiceName} with price ₱{serviceTicket.Price}",
                    Timestamp = DateTime.Now
                };
                _context.ActivityLogs.Add(log);
                // --- LOGGING END ---

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceTicket);
        }

        // GET: ServiceTickets/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceTicket = await _context.ServiceTicket.FindAsync(id);
            if (serviceTicket == null) return NotFound();

            return View(serviceTicket);
        }

        // POST: ServiceTickets/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceName,Description,Price,ImageUrl,Category,CreatedAt,IsActive")] ServiceTicket serviceTicket)
        {
            if (id != serviceTicket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceTicket);

                    // --- LOGGING START ---
                    var log = new ActivityLog
                    {
                        UserEmail = User.Identity.Name ?? "System",
                        Action = "Service Edited",
                        Details = $"Updated details for service: {serviceTicket.ServiceName}",
                        Timestamp = DateTime.Now
                    };
                    _context.ActivityLogs.Add(log);
                    // --- LOGGING END ---

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceTicketExists(serviceTicket.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceTicket);
        }

        // GET: ServiceTickets/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceTicket = await _context.ServiceTicket
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceTicket == null) return NotFound();

            return View(serviceTicket);
        }

        // POST: ServiceTickets/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceTicket = await _context.ServiceTicket.FindAsync(id);
            if (serviceTicket != null)
            {
                // --- LOGGING START ---
                var log = new ActivityLog
                {
                    UserEmail = User.Identity.Name ?? "System",
                    Action = "Service Deleted",
                    Details = $"Removed service: {serviceTicket.ServiceName}",
                    Timestamp = DateTime.Now
                };
                _context.ActivityLogs.Add(log);
                // --- LOGGING END ---

                _context.ServiceTicket.Remove(serviceTicket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceTicketExists(int id)
        {
            return _context.ServiceTicket.Any(e => e.Id == id);
        }
    }
}