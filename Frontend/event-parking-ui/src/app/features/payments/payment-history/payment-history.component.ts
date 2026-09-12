import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  PaymentHistory
} from '../../../core/models/payment.model';

import {
  PaymentService
} from '../../../core/services/payment.service';

import {
  AuthService
} from '../../../core/services/auth.service';

@Component({
  selector: 'app-payment-history',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './payment-history.component.html',
  styleUrl: './payment-history.component.css'
})
export class PaymentHistoryComponent implements OnInit {

  customerId = 0;

  payments: PaymentHistory[] = [];

  isLoading = false;

  errorMessage = '';

  constructor(
    private paymentService: PaymentService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {

    const currentUser =
      this.authService.getCurrentUser();

    if (!currentUser) {

      this.router.navigate([
        '/login'
      ]);

      return;
    }

    this.customerId =
      currentUser.customerId;

    this.loadPaymentHistory();
  }

  loadPaymentHistory(): void {

    if (!this.customerId) {

      this.errorMessage =
        'Customer information is not available.';

      return;
    }

    this.isLoading = true;

    this.errorMessage = '';

    this.paymentService
      .getCustomerPayments(
        this.customerId
      )
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success) {

            this.payments =
              response.data ?? [];

          } else {

            this.payments = [];

            this.errorMessage =
              response.message ||
              'Unable to load payment history.';
          }

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.isLoading = false;

          this.payments = [];

          this.errorMessage =
            error?.error?.message ||
            'Unable to load payment history.';

          this.cdr.markForCheck();
        }

      });
  }

  viewReceipt(
    paymentId: number
  ): void {

    this.router.navigate([
      '/payments',
      paymentId,
      'receipt'
    ]);
  }

  getStatusClass(
    status: string
  ): string {

    return status
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
}
