using Event_parking.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_parking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ============================
        // MEMBER 2 TABLES
        // ============================

        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<Event> Events { get; set; }
    }
}