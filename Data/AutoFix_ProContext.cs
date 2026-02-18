using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoFix_Pro.Models;

namespace AutoFix_Pro.Data
{
    public class AutoFix_ProContext : DbContext
    {
        public AutoFix_ProContext (DbContextOptions<AutoFix_ProContext> options)
            : base(options)
        {
        }

        public DbSet<AutoFix_Pro.Models.ServiceTicket> ServiceTicket { get; set; } = default!;
    }
}
