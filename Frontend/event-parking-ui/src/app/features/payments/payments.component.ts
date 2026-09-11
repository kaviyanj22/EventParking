import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { PaymentHistory } from '../../core/models/payment.model';
import { PaymentService } from '../../core/services/payment.service';

@Component({
  selector: 'app-admin-payments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payments.component.html',
  styleUrl: './payments.component.css'
})
export class AdminPaymentsComponent implements OnInit {

  payments: PaymentHistory[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private paymentService: PaymentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPayments();
  }

  loadPayments(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.paymentService
      .getAllPayments()
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success) {

            this.payments = response.data ?? [];

          } else {

            this.payments = [];

            this.errorMessage =
              response.message ||
              'Unable to load payments.';
          }
        },

        error: (error) => {

          this.isLoading = false;
          this.payments = [];

          this.errorMessage =
            error.error?.message ||
            'Unable to load payments.';
        }
      });
  }

  viewReceipt(paymentId: number): void {
    this.router.navigate([
      '/payments',
      paymentId,
      'receipt'
    ]);
  }

  getStatusClass(status: string): string {
    return status
      .toLowerCase()
      .replace(/\s+/g, '-');
  }

  get totalRevenue(): number {
    return this.payments.reduce(
      (total, payment) =>
        total + payment.amount,
      0
    );
  }
}