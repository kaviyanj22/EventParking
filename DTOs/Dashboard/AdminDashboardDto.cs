namespace Event_parking.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalEvents { get; set; }

        public int TotalBookings { get; set; }

        public int AvailableSeats { get; set; }

        public int OccupiedParkingSlots { get; set; }

        public decimal TotalRevenueCollected { get; set; }

        public int TotalCustomers { get; set; }
    }
}