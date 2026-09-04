using Event_parking.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        // MEMBER 1 TABLES
        // ============================

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        // ============================
        // MEMBER 2 TABLES
        // ============================

        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }


        public DbSet<Event> Events { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        public DbSet<ParkingReservation> ParkingReservations { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(customer => customer.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(vehicle => vehicle.VehicleNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasOne(vehicle => vehicle.Customer)
                .WithMany(customer => customer.Vehicles)
                .HasForeignKey(vehicle => vehicle.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}