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

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

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