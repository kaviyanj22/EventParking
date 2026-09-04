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

        // ======================================
        // MEMBER 1 TABLES
        // ======================================

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        // ======================================
        // MEMBER 2 TABLES
        // ======================================

        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<Event> Events { get; set; }

        // ======================================
        // MEMBER 3 TABLES
        // ======================================

        public DbSet<Seat> Seats { get; set; }

        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        public DbSet<ParkingReservation>
            ParkingReservations
        { get; set; }

        // ======================================
        // MEMBER 4 TABLES
        // ======================================

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<BookingSeat> BookingSeats { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================
            // MEMBER 1 CONFIGURATION
            // ======================================

            modelBuilder.Entity<Customer>()
                .HasIndex(customer =>
                    customer.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(vehicle =>
                    vehicle.VehicleNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasOne(vehicle =>
                    vehicle.Customer)
                .WithMany(customer =>
                    customer.Vehicles)
                .HasForeignKey(vehicle =>
                    vehicle.CustomerId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // ======================================
            // MEMBER 4 - BOOKING
            // ======================================

            modelBuilder.Entity<Booking>()
                .HasIndex(booking =>
                    booking.BookingNumber)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasOne(booking =>
                    booking.Customer)
                .WithMany()
                .HasForeignKey(booking =>
                    booking.CustomerId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(booking =>
                    booking.Event)
                .WithMany()
                .HasForeignKey(booking =>
                    booking.EventId)
                .OnDelete(
                    DeleteBehavior.Restrict);


            // ======================================
            // MEMBER 4 - BOOKING SEAT
            // ======================================

            modelBuilder.Entity<Booking>()
                .HasMany(booking =>
                    booking.BookingSeats)
                .WithOne()
                .HasForeignKey(bookingSeat =>
                    bookingSeat.BookingId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bookingSeat =>
                    bookingSeat.Seat)
                .WithMany(seat =>
                    seat.BookingSeats)
                .HasForeignKey(bookingSeat =>
                    bookingSeat.SeatId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            // One seat can have only one
            // ACTIVE booking at a time
            modelBuilder.Entity<BookingSeat>()
                .HasIndex(bookingSeat =>
                    bookingSeat.SeatId)
                .IsUnique()
                .HasFilter(
                    "[IsActive] = 1");


            // ======================================
            // MEMBER 4 - PARKING RESERVATION
            // ======================================

            modelBuilder.Entity<ParkingReservation>()
                .HasOne(reservation =>
                    reservation.Booking)
                .WithOne(booking =>
                    booking.ParkingReservation)
                .HasForeignKey<ParkingReservation>(
                    reservation =>
                        reservation.BookingId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<ParkingReservation>()
                .HasOne(reservation =>
                    reservation.ParkingSlot)
                .WithMany(parkingSlot =>
                    parkingSlot.ParkingReservations)
                .HasForeignKey(reservation =>
                    reservation.ParkingSlotId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            // One parking slot can have only one
            // ACTIVE reservation at a time
            modelBuilder.Entity<ParkingReservation>()
                .HasIndex(reservation =>
                    reservation.ParkingSlotId)
                .IsUnique()
                .HasFilter(
                    "[IsActive] = 1");


            // ======================================
            // MEMBER 4 - PAYMENT
            // ======================================

            modelBuilder.Entity<Payment>()
                .HasOne(payment =>
                    payment.Booking)
                .WithOne(booking =>
                    booking.Payment)
                .HasForeignKey<Payment>(
                    payment =>
                        payment.BookingId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(payment =>
                    payment.Customer)
                .WithMany()
                .HasForeignKey(payment =>
                    payment.CustomerId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            // Extra DB protection:
            // only one payment per booking
            modelBuilder.Entity<Payment>()
                .HasIndex(payment =>
                    payment.BookingId)
                .IsUnique();


            // ======================================
            // MEMBER 4 - NOTIFICATION
            // ======================================

            modelBuilder.Entity<Notification>()
                .HasOne(notification =>
                    notification.Customer)
                .WithMany()
                .HasForeignKey(notification =>
                    notification.CustomerId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(notification =>
                    notification.Booking)
                .WithMany()
                .HasForeignKey(notification =>
                    notification.BookingId)
                .OnDelete(
                    DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(notification =>
                    notification.Event)
                .WithMany()
                .HasForeignKey(notification =>
                    notification.EventId)
                .OnDelete(
                    DeleteBehavior.NoAction);
        }
    }
}