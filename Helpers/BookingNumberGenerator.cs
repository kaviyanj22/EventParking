namespace Event_parking.Helpers
{
    public class BookingNumberGenerator
    {
        public string Generate()
        {
            string year =
                DateTime.UtcNow.Year.ToString();

            string uniquePart =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            return $"BKG-{year}-{uniquePart}";
        }
    }
}