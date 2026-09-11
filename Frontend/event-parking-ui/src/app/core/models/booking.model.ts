export interface BookingCreate {
  eventId: number;
  seatIds: number[];
  parkingSlotId?: number | null;
}

export interface BookingSeat {
  seatId: number;
  seatNumber: string;
  rowName?: string | null;
  columnNumber?: number | null;
  seatType?: string | null;
  priceAtBooking: number;
}

export interface BookingParking {
  parkingSlotId: number;
  slotNumber: string;
  zone?: string | null;
  feeAtReservation: number;
}

export interface Booking {
  bookingId: number;
  bookingNumber: string;

  customerId: number;
  customerName: string;

  eventId: number;
  eventName: string;

  status: string;

  holdExpiresAt?: string | null;

  seats: BookingSeat[];

  parking?: BookingParking | null;

  seatTotal: number;
  parkingFee: number;
  totalAmount: number;

  createdAt: string;

  confirmedAt?: string | null;
  cancelledAt?: string | null;
}