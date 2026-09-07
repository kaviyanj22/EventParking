using Event_parking.DTOs.Payment;

namespace Event_parking.Services.Interfaces
{
    public interface IPaymentService
    {
        // ======================================
        // GET PAYMENT SUMMARY
        // ======================================

        Task<ServiceResult<PaymentSummaryDto>>
            GetPaymentSummaryAsync(
                int bookingId,
                int customerId,
                bool isAdmin
            );

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
        // ALL PAYMENTS
        // ADMIN
        // ======================================

        Task<ServiceResult<List<PaymentHistoryDto>>>
            GetAllPaymentsAsync();

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