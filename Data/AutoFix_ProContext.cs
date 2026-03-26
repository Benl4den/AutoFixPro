using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AutoFix_Pro.Data
{
    public class AutoFix_ProContext : IdentityDbContext
    {
        public AutoFix_ProContext(DbContextOptions<AutoFix_ProContext> options)
            : base(options)
        {
        }

        // This links your ServiceTicket model to the database table
        public DbSet<AutoFix_Pro.Models.ServiceTicket> ServiceTicket { get; set; } = default!;

        public DbSet<AutoFix_Pro.Models.Appointment> Appointments { get; set; } = default!;
        public DbSet<AutoFix_Pro.Models.ActivityLog> ActivityLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This ensures the context knows to use SQLite if not already configured in Program.cs
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=AutoFixPro.db");
            }
        }
    }
}