namespace Event_parking.Configurations
{
    public class BookingSettings
    {
        public const string SectionName = "BookingSettings";

        public int HoldMinutes { get; set; } = 15;

        public int ExpiryCheckIntervalSeconds { get; set; } = 60;
    }
}