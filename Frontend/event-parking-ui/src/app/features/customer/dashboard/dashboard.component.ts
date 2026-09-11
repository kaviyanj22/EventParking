import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { Booking } from '../../../core/models/booking.model';
import { PaymentHistory } from '../../../core/models/payment.model';
import { Notification } from '../../../core/models/notification.model';

import { BookingService } from '../../../core/services/booking.service';
import { PaymentService } from '../../../core/services/payment.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class CustomerDashboardComponent implements OnChanges {

  @Input() customerId: number | null = null;

  bookings: Booking[] = [];
  payments: PaymentHistory[] = [];
  notifications: Notification[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private bookingService: BookingService,
    private paymentService: PaymentService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['customerId'] && this.customerId) {
      this.loadDashboard();
    }
  }

  loadDashboard(): void {

    if (!this.customerId) {
      this.errorMessage =
        'Customer information is not available.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.loadBookings();
    this.loadPayments();
    this.loadNotifications();
  }

  private loadBookings(): void {

    this.bookingService
      .getCustomerBookings(this.customerId!)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.bookings = response.data ?? [];
          }

          this.finishLoading();
        },
        error: () => {
          this.finishLoading();
        }
      });
  }

  private loadPayments(): void {

    this.paymentService
      .getCustomerPayments(this.customerId!)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.payments = response.data ?? [];
          }

          this.finishLoading();
        },
        error: () => {
          this.finishLoading();
        }
      });
  }

  private loadNotifications(): void {

    this.notificationService
      .getCustomerNotifications(this.customerId!)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notifications = response.data ?? [];
          }

          this.finishLoading();
        },
        error: () => {
          this.finishLoading();
        }
      });
  }

  private completedRequests = 0;

  private finishLoading(): void {

    this.completedRequests++;

    if (this.completedRequests >= 3) {
      this.isLoading = false;
      this.completedRequests = 0;
    }
  }

  get confirmedBookings(): number {
    return this.bookings.filter(
      booking =>
        booking.status.toLowerCase() === 'confirmed'
    ).length;
  }

  get unreadNotifications(): number {
    return this.notifications.filter(
      notification => !notification.isRead
    ).length;
  }

  get totalSpent(): number {
    return this.payments.reduce(
      (total, payment) => total + payment.amount,
      0
    );
  }

  goToBookings(): void {
    this.router.navigate(['/my-bookings']);
  }

  goToPayments(): void {
    this.router.navigate(['/payment-history']);
  }

  goToNotifications(): void {
    this.router.navigate(['/notifications']);
  }

  goToEvents(): void {
    this.router.navigate(['/events']);
  }
}