using Event_parking.DTOs.Notification;
using Event_parking.DTOs.Payment;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;

namespace Event_parking.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _notificationService = notificationService;
        }

        // ======================================
        // GET PAYMENT BY BOOKING
        // ======================================

        public async Task<ServiceResult<PaymentResponseDto>>
            GetPaymentByBookingIdAsync(
                int bookingId,
                int customerId,
                bool isAdmin)
        {
            Booking? booking =
                await _paymentRepository
                    .GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
            {
                return ServiceResult<PaymentResponseDto>
                    .Fail("Booking was not found.");
            }

            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                return ServiceResult<PaymentResponseDto>
                    .Fail(
                        "You are not authorized to access this booking.");
            }

            Payment? payment =
                await _paymentRepository
                    .GetPaymentByBookingIdAsync(bookingId);

            if (payment == null)
            {
                return ServiceResult<PaymentResponseDto>
                    .Fail("Payment was not found.");
            }

            return ServiceResult<PaymentResponseDto>.Ok(
                MapToPaymentResponseDto(
                    payment,
                    booking.BookingNumber),
                "Payment retrieved successfully.");
        }

        // ======================================
        // CREATE PAYMENT
        // ======================================

        public async Task<ServiceResult<PaymentResponseDto>>
            CreatePaymentAsync(
                int bookingId,
                int customerId)
        {
            await using var transaction =
                await _paymentRepository
                    .BeginTransactionAsync();

            try
            {
                Booking? booking =
                    await _paymentRepository
                        .GetBookingWithDetailsAsync(
                            bookingId);

                if (booking == null)
                {
                    return ServiceResult<PaymentResponseDto>
                        .Fail("Booking was not found.");
                }

                if (booking.CustomerId != customerId)
                {
                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "You are not authorized to pay for this booking.");
                }

                if (!string.Equals(
                    booking.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "Only pending bookings can be paid.");
                }

                // ======================================
                // HOLD EXPIRY CHECK
                // ======================================

                DateTime utcNow = DateTime.UtcNow;

                if (booking.HoldExpiresAt.HasValue &&
                    booking.HoldExpiresAt.Value <= utcNow)
                {
                    ExpireBooking(booking, utcNow);

                    await _paymentRepository
                        .SaveChangesAsync();

                    await transaction.CommitAsync();

                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "Booking hold has expired. Payment cannot be completed.");
                }

                // ======================================
                // ONE PAYMENT PER BOOKING
                // ======================================

                Payment? existingPayment =
                    await _paymentRepository
                        .GetPaymentByBookingIdAsync(
                            bookingId);

                if (existingPayment != null)
                {
                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "A payment already exists for this booking.");
                }

                // ======================================
                // CALCULATE TOTAL
                // ======================================

                decimal seatTotal =
                    booking.BookingSeats
                        .Where(bookingSeat =>
                            bookingSeat.IsActive)
                        .Sum(bookingSeat =>
                            bookingSeat.PriceAtBooking);

                decimal parkingFee = 0m;

                if (booking.ParkingReservation != null &&
                    booking.ParkingReservation.IsActive)
                {
                    parkingFee =
                        booking.ParkingReservation
                            .FeeAtReservation;
                }

                decimal totalAmount =
                    seatTotal + parkingFee;

                if (totalAmount <= 0)
                {
                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "The booking total amount is invalid.");
                }

                // ======================================
                // CREATE PAYMENT
                // ======================================

                Payment payment =
                    new Payment
                    {
                        BookingId =
                            booking.BookingId,

                        CustomerId =
                            booking.CustomerId,

                        Amount =
                            totalAmount,

                        Status =
                            "Completed",

                        TransactionReference =
                            GenerateTransactionReference(),

                        PaidAt =
                            utcNow,

                        CreatedAt =
                            utcNow
                    };

                await _paymentRepository
                    .AddPaymentAsync(payment);

                // ======================================
                // CONFIRM BOOKING
                // ======================================

                booking.Status = "Confirmed";
                booking.ConfirmedAt = utcNow;
                booking.UpdatedAt = utcNow;

                bool saved =
                    await _paymentRepository
                        .SaveChangesAsync();

                if (!saved)
                {
                    await transaction.RollbackAsync();

                    return ServiceResult<PaymentResponseDto>
                        .Fail(
                            "Failed to complete payment.");
                }

                await transaction.CommitAsync();

                // ======================================
                // PAYMENT SUCCESSFUL NOTIFICATION
                // ======================================

                await _notificationService
                    .CreateNotificationAsync(
                        new NotificationCreateDto
                        {
                            CustomerId =
                                booking.CustomerId,

                            BookingId =
                                booking.BookingId,

                            EventId =
                                booking.EventId,

                            Type =
                                "PaymentSuccessful",

                            Title =
                                "Payment Successful",

                            Message =
                                $"Payment for booking {booking.BookingNumber} was successful."
                        });

                // ======================================
                // BOOKING CONFIRMED NOTIFICATION
                // ======================================

                await _notificationService
                    .CreateNotificationAsync(
                        new NotificationCreateDto
                        {
                            CustomerId =
                                booking.CustomerId,

                            BookingId =
                                booking.BookingId,

                            EventId =
                                booking.EventId,

                            Type =
                                "BookingConfirmed",

                            Title =
                                "Booking Confirmed",

                            Message =
                                $"Booking {booking.BookingNumber} has been confirmed."
                        });

                return ServiceResult<PaymentResponseDto>.Ok(
                    MapToPaymentResponseDto(
                        payment,
                        booking.BookingNumber),
                    "Payment completed successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();

                return ServiceResult<PaymentResponseDto>
                    .Fail(
                        "An error occurred while processing the payment.");
            }
        }

        // ======================================
        // CUSTOMER PAYMENT HISTORY
        // ======================================

        public async Task<
            ServiceResult<List<PaymentHistoryDto>>>
            GetCustomerPaymentsAsync(
                int customerId)
        {
            List<Payment> payments =
                await _paymentRepository
                    .GetPaymentsByCustomerAsync(
                        customerId);

            List<PaymentHistoryDto> response =
                payments
                    .Select(payment =>
                        new PaymentHistoryDto
                        {
                            PaymentId =
                                payment.PaymentId,

                            BookingId =
                                payment.BookingId,

                            BookingNumber =
                                payment.Booking?
                                    .BookingNumber
                                ?? string.Empty,

                            EventName =
                                payment.Booking?
                                    .Event?
                                    .EventName
                                ?? string.Empty,

                            Amount =
                                payment.Amount,

                            Status =
                                payment.Status,

                            TransactionReference =
                                payment.TransactionReference,

                            PaidAt =
                                payment.PaidAt
                        })
                    .ToList();

            return ServiceResult<
                List<PaymentHistoryDto>>.Ok(
                    response,
                    "Payment history retrieved successfully.");
        }

        // ======================================
        // GET RECEIPT
        // ======================================

        public async Task<ServiceResult<ReceiptDto>>
            GetReceiptAsync(
                int paymentId,
                int customerId,
                bool isAdmin)
        {
            Payment? payment =
                await _paymentRepository
                    .GetPaymentByIdAsync(
                        paymentId);

            if (payment == null)
            {
                return ServiceResult<ReceiptDto>
                    .Fail("Payment was not found.");
            }

            if (!isAdmin &&
                payment.CustomerId != customerId)
            {
                return ServiceResult<ReceiptDto>
                    .Fail(
                        "You are not authorized to access this receipt.");
            }

            if (payment.Booking == null)
            {
                return ServiceResult<ReceiptDto>
                    .Fail(
                        "Booking information was not found.");
            }

            Booking booking =
                payment.Booking;

            decimal seatTotal =
                booking.BookingSeats
                    .Sum(bookingSeat =>
                        bookingSeat.PriceAtBooking);

            decimal parkingFee =
                booking.ParkingReservation?
                    .FeeAtReservation
                ?? 0m;

            ReceiptDto receipt =
                new ReceiptDto
                {
                    PaymentId =
                        payment.PaymentId,

                    TransactionReference =
                        payment.TransactionReference
                        ?? string.Empty,

                    BookingId =
                        booking.BookingId,

                    BookingNumber =
                        booking.BookingNumber,

                    CustomerName =
                        payment.Customer?
                            .FullName
                        ?? string.Empty,

                    CustomerEmail =
                        payment.Customer?
                            .Email
                        ?? string.Empty,

                    EventName =
                        booking.Event?
                            .EventName
                        ?? string.Empty,

                    SeatTotal =
                        seatTotal,

                    ParkingFee =
                        parkingFee,

                    TotalAmount =
                        payment.Amount,

                    PaymentStatus =
                        payment.Status,

                    PaidAt =
                        payment.PaidAt
                };

            return ServiceResult<ReceiptDto>.Ok(
                receipt,
                "Receipt retrieved successfully.");
        }

        // ======================================
        // EXPIRE BOOKING + RELEASE RESOURCES
        // ======================================

        private static void ExpireBooking(
            Booking booking,
            DateTime utcNow)
        {
            booking.Status = "Expired";
            booking.UpdatedAt = utcNow;

            foreach (BookingSeat bookingSeat
                     in booking.BookingSeats)
            {
                if (!bookingSeat.IsActive)
                {
                    continue;
                }

                bookingSeat.IsActive = false;
                bookingSeat.ReleasedAt = utcNow;

                if (bookingSeat.Seat != null)
                {
                    bookingSeat.Seat.Status =
                        "Available";

                    bookingSeat.Seat.UpdatedAt =
                        utcNow;
                }
            }

            if (booking.ParkingReservation != null &&
                booking.ParkingReservation.IsActive)
            {
                booking.ParkingReservation.IsActive =
                    false;

                booking.ParkingReservation.ReleasedAt =
                    utcNow;

                if (booking.ParkingReservation
                    .ParkingSlot != null)
                {
                    booking.ParkingReservation
                        .ParkingSlot!
                        .Status =
                        "Available";

                    booking.ParkingReservation
                        .ParkingSlot!
                        .UpdatedAt =
                        utcNow;
                }
            }
        }

        // ======================================
        // TRANSACTION REFERENCE GENERATOR
        // ======================================

        private static string
            GenerateTransactionReference()
        {
            string uniquePart =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 10)
                    .ToUpperInvariant();

            return $"TXN-{DateTime.UtcNow.Year}-{uniquePart}";
        }

        // ======================================
        // PAYMENT MAPPER
        // ======================================

        private static PaymentResponseDto
            MapToPaymentResponseDto(
                Payment payment,
                string bookingNumber)
        {
            return new PaymentResponseDto
            {
                PaymentId =
                    payment.PaymentId,

                BookingId =
                    payment.BookingId,

                BookingNumber =
                    bookingNumber,

                CustomerId =
                    payment.CustomerId,

                Amount =
                    payment.Amount,

                Status =
                    payment.Status,

                TransactionReference =
                    payment.TransactionReference,

                PaidAt =
                    payment.PaidAt,

                CreatedAt =
                    payment.CreatedAt
            };
        }
    }
}