export interface Payment {
  paymentId: number;
  bookingId: number;
  bookingNumber: string;
  customerId: number;
  amount: number;
  status: string;
  transactionReference?: string | null;
  paidAt: string;
  createdAt: string;
}

export interface PaymentSummary {
  bookingId: number;
  bookingNumber: string;
  eventName: string;

  seatTotal: number;
  parkingFee: number;
  totalAmount: number;

  bookingStatus: string;
  paymentStatus: string;

  holdExpiresAt?: string | null;
  remainingSeconds: number;
  isExpired: boolean;
}

export interface PaymentHistory {
  paymentId: number;
  bookingId: number;
  bookingNumber: string;
  eventName: string;

  amount: number;
  status: string;

  transactionReference?: string | null;
  paidAt: string;
}

export interface Receipt {
  paymentId: number;
  transactionReference: string;

  bookingId: number;
  bookingNumber: string;

  customerName: string;
  customerEmail: string;

  eventName: string;

  seatTotal: number;
  parkingFee: number;
  totalAmount: number;

  paymentStatus: string;
  paidAt: string;
}