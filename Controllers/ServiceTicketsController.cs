using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Data;
using AutoFix_Pro.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceTicket.ToListAsync());
        }

        // GET: ServiceTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceTicket = await _context.ServiceTicket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceTicket == null)
            {
                return NotFound();
            }

            return View(serviceTicket);
        }

        // GET: ServiceTickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientName,PlateNumber,ServiceType,JobDate,Price")] ServiceTicket serviceTicket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceTicket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceTicket);
        }

        // GET: ServiceTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceTicket = await _context.ServiceTicket.FindAsync(id);
            if (serviceTicket == null)
            {
                return NotFound();
            }
            return View(serviceTicket);
        }

        // POST: ServiceTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientName,PlateNumber,ServiceType,JobDate,Price")] ServiceTicket serviceTicket)
        {
            if (id != serviceTicket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceTicket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceTicketExists(serviceTicket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceTicket);
        }

        // GET: ServiceTickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceTicket = await _context.ServiceTicket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceTicket == null)
            {
                return NotFound();
            }

            return View(serviceTicket);
        }

        // POST: ServiceTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceTicket = await _context.ServiceTicket.FindAsync(id);
            if (serviceTicket != null)
            {
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
