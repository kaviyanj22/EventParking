import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import {
  Payment,
  PaymentSummary
} from '../../../core/models/payment.model';

import {
  PaymentService
} from '../../../core/services/payment.service';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.css'
})
export class PaymentComponent implements OnInit {

  bookingId = 0;

  paymentSummary: PaymentSummary | null = null;
  payment: Payment | null = null;

  isLoading = true;
  isPaying = false;

  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService
  ) {}

  ngOnInit(): void {

    this.bookingId =
      Number(this.route.snapshot.paramMap.get('id'));

    if (!this.bookingId) {
      this.isLoading = false;
      this.errorMessage = 'Invalid booking.';
      return;
    }

    this.loadPaymentSummary();
  }

  loadPaymentSummary(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.paymentService
      .getPaymentSummary(this.bookingId)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success && response.data) {

            this.paymentSummary = response.data;

            if (
              response.data.isExpired ||
              response.data.remainingSeconds <= 0
            ) {
              this.errorMessage =
                'Booking hold has expired. Payment cannot be completed.';
            }

          } else {

            this.errorMessage =
              response.message ||
              'Unable to load payment summary.';
          }
        },

        error: (error) => {

          this.isLoading = false;

          this.errorMessage =
            error.error?.message ||
            'Unable to load payment summary.';
        }
      });
  }

  payNow(): void {

    if (
      !this.paymentSummary ||
      this.paymentSummary.isExpired ||
      this.paymentSummary.remainingSeconds <= 0 ||
      this.isPaying
    ) {
      return;
    }

    this.isPaying = true;
    this.errorMessage = '';

    this.paymentService
      .createPayment(this.bookingId)
      .subscribe({

        next: (response) => {

          this.isPaying = false;

          if (response.success && response.data) {

            this.payment = response.data;

            this.router.navigate([
              '/bookings',
              this.bookingId,
              'confirmation'
            ]);

          } else {

            this.errorMessage =
              response.message ||
              'Payment could not be completed.';
          }
        },

        error: (error) => {

          this.isPaying = false;

          if (error.status === 409) {
            this.errorMessage =
              error.error?.message ||
              'Payment has already been completed or the booking is no longer available.';
            return;
          }

          this.errorMessage =
            error.error?.message ||
            'Payment could not be completed.';
        }
      });
  }

  get canPay(): boolean {

    return !!(
      this.paymentSummary &&
      !this.paymentSummary.isExpired &&
      this.paymentSummary.remainingSeconds > 0 &&
      !this.isPaying
    );
  }

  goBack(): void {

    this.router.navigate([
      '/bookings',
      this.bookingId,
      'hold'
    ]);
  }
}