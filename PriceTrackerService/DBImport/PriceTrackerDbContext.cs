using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PriceTrackerService.DBImport
{
    public class PriceTrackerDbContext : DbContext
    {
        public PriceTrackerDbContext()
        {
        }
        public PriceTrackerDbContext(DbContextOptions<PriceTrackerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PriceSubscription> PriceSubscriptions { get; set; }
    }
}