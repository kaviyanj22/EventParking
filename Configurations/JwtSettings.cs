namespace Event_parking.Configurations
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int ExpiryMinutes { get; set; } = 60;
    }
}