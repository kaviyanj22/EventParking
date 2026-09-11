import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Booking,
  BookingCreate
} from '../models/booking.model';

export interface ServiceResult<T> {
  success: boolean;
  message: string;
  data: T | null;
}

export interface BookingHoldStatus {
  bookingId: number;
  bookingNumber: string;
  status: string;
  holdExpiresAt?: string | null;
  remainingSeconds: number;
  isExpired: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {

private readonly apiUrl = `${environment.apiUrl}/bookings`;
  constructor(private http: HttpClient) {}

  createBooking(
    booking: BookingCreate
  ): Observable<ServiceResult<Booking>> {
    return this.http.post<ServiceResult<Booking>>(
      this.apiUrl,
      booking
    );
  }

  getCustomerBookings(
    customerId: number
  ): Observable<ServiceResult<Booking[]>> {
    return this.http.get<ServiceResult<Booking[]>>(
      `${this.apiUrl}/customer/${customerId}`
    );
  }

  getBookingById(
    bookingId: number
  ): Observable<ServiceResult<Booking>> {
    return this.http.get<ServiceResult<Booking>>(
      `${this.apiUrl}/${bookingId}`
    );
  }

  getHoldStatus(
    bookingId: number
  ): Observable<ServiceResult<BookingHoldStatus>> {
    return this.http.get<ServiceResult<BookingHoldStatus>>(
      `${this.apiUrl}/${bookingId}/hold-status`
    );
  }

  cancelBooking(
    bookingId: number
  ): Observable<ServiceResult<boolean>> {
    return this.http.delete<ServiceResult<boolean>>(
      `${this.apiUrl}/${bookingId}`
    );
  }

  getAllBookings(
    eventId?: number
  ): Observable<ServiceResult<Booking[]>> {

    const url = eventId
      ? `${this.apiUrl}?eventId=${eventId}`
      : this.apiUrl;

    return this.http.get<ServiceResult<Booking[]>>(url);
  }
}