import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Payment,
  PaymentSummary,
  PaymentHistory,
  Receipt
} from '../models/payment.model';

import { ServiceResult } from './booking.service';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

 private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPaymentSummary(
    bookingId: number
  ): Observable<ServiceResult<PaymentSummary>> {
    return this.http.get<ServiceResult<PaymentSummary>>(
      `${this.apiUrl}/bookings/${bookingId}/payment`
    );
  }

  createPayment(
    bookingId: number
  ): Observable<ServiceResult<Payment>> {
    return this.http.post<ServiceResult<Payment>>(
      `${this.apiUrl}/bookings/${bookingId}/payment`,
      {}
    );
  }

  getAllPayments():
    Observable<ServiceResult<PaymentHistory[]>> {
    return this.http.get<ServiceResult<PaymentHistory[]>>(
      `${this.apiUrl}/payments`
    );
  }

  getCustomerPayments(
    customerId: number
  ): Observable<ServiceResult<PaymentHistory[]>> {
    return this.http.get<ServiceResult<PaymentHistory[]>>(
      `${this.apiUrl}/payments/customer/${customerId}`
    );
  }

  getReceipt(
    paymentId: number
  ): Observable<ServiceResult<Receipt>> {
    return this.http.get<ServiceResult<Receipt>>(
      `${this.apiUrl}/payments/${paymentId}/receipt`
    );
  }
}