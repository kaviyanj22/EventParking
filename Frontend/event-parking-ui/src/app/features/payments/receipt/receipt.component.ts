import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Receipt } from '../../../core/models/payment.model';
import { PaymentService } from '../../../core/services/payment.service';

@Component({
  selector: 'app-receipt',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './receipt.component.html',
  styleUrl: './receipt.component.css'
})
export class ReceiptComponent implements OnInit {

  paymentId = 0;

  receipt: Receipt | null = null;

  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService
  ) {}

  ngOnInit(): void {

    this.paymentId =
      Number(this.route.snapshot.paramMap.get('id'));

    if (!this.paymentId) {
      this.isLoading = false;
      this.errorMessage = 'Invalid payment.';
      return;
    }

    this.loadReceipt();
  }

  loadReceipt(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.paymentService
      .getReceipt(this.paymentId)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success && response.data) {

            this.receipt = response.data;

          } else {

            this.errorMessage =
              response.message ||
              'Unable to load receipt.';
          }
        },

        error: (error) => {

          this.isLoading = false;

          this.errorMessage =
            error.error?.message ||
            'Unable to load receipt.';
        }
      });
  }

  printReceipt(): void {
    window.print();
  }

  goToPaymentHistory(): void {
    this.router.navigate(['/payment-history']);
  }
}