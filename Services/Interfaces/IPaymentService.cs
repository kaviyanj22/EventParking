using Event_parking.DTOs.Payment;

namespace Event_parking.Services.Interfaces
{
    public interface IPaymentService
    {
        // ======================================
        // GET PAYMENT BY BOOKING
        // ======================================

        Task<ServiceResult<PaymentResponseDto>>
            GetPaymentByBookingIdAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

        // ======================================
        // CREATE PAYMENT
        // ======================================

        Task<ServiceResult<PaymentResponseDto>>
            CreatePaymentAsync(
                int bookingId,
                int customerId
            );

        // ======================================
        // CUSTOMER PAYMENT HISTORY
        // ======================================

        Task<ServiceResult<List<PaymentHistoryDto>>>
            GetCustomerPaymentsAsync(
                int customerId
            );

        // ======================================
        // RECEIPT
        // ======================================

        Task<ServiceResult<ReceiptDto>>
            GetReceiptAsync(
                int paymentId,
                int customerId,
                bool isAdmin
            );
    }
}