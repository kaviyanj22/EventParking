using Event_parking.Configurations;
using Event_parking.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Event_parking.BackgroundServices
{
    public class BookingExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly BookingSettings _bookingSettings;
        private readonly ILogger<BookingExpiryService> _logger;

        public BookingExpiryService(
            IServiceScopeFactory scopeFactory,
            IOptions<BookingSettings> bookingSettings,
            ILogger<BookingExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _bookingSettings = bookingSettings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            int intervalSeconds =
                _bookingSettings.ExpiryCheckIntervalSeconds > 0
                    ? _bookingSettings.ExpiryCheckIntervalSeconds
                    : 60;

            using PeriodicTimer timer =
                new PeriodicTimer(
                    TimeSpan.FromSeconds(intervalSeconds));

            try
            {
                while (await timer.WaitForNextTickAsync(
                    stoppingToken))
                {
                    try
                    {
                        using IServiceScope scope =
                            _scopeFactory.CreateScope();

                        IBookingService bookingService =
                            scope.ServiceProvider
                                .GetRequiredService<IBookingService>();

                        await bookingService
                            .ExpirePendingBookingsAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "An error occurred while expiring pending bookings.");
                    }
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                // Application is shutting down normally.
            }
        }
    }
}