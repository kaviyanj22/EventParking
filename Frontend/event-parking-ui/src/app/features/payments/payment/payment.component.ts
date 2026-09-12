import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

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

  imports: [
    CommonModule,
    ReactiveFormsModule
  ],

  templateUrl: './payment.component.html',
  styleUrl: './payment.component.css'
})
export class PaymentComponent
  implements OnInit, OnDestroy {

  bookingId = 0;

  paymentSummary: PaymentSummary | null = null;

  payment: Payment | null = null;

  isLoading = true;

  isPaying = false;

  errorMessage = '';

  private countdownTimer:
    ReturnType<typeof setInterval> | null = null;

  // =========================================
  // FAKE ATM / DEBIT CARD FORM
  // =========================================

  paymentForm = new FormGroup({

    cardNumber:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            this.cardNumberValidator()
          ]
        }
      ),

    cardholderName:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100)
          ]
        }
      ),

    expiryDate:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            this.expiryDateValidator()
          ]
        }
      ),

    cvv:
      new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.pattern(/^[0-9]{3}$/)
          ]
        }
      )

  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService,
    private cdr: ChangeDetectorRef
  ) {}

  // =========================================
  // INIT
  // =========================================

  ngOnInit(): void {

    this.bookingId =
      Number(
        this.route
          .snapshot
          .paramMap
          .get('id')
      );

    if (!this.bookingId) {

      this.isLoading = false;

      this.errorMessage =
        'Invalid booking.';

      this.cdr.detectChanges();

      return;
    }

    this.loadPaymentSummary();
  }

  // =========================================
  // LOAD PAYMENT SUMMARY
  // =========================================

  loadPaymentSummary(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.paymentService
      .getPaymentSummary(
        this.bookingId
      )
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (
            response.success &&
            response.data
          ) {

            this.paymentSummary =
              response.data;

            if (
              response.data.isExpired ||
              response.data.remainingSeconds <= 0
            ) {

              this.paymentSummary.isExpired =
                true;

              this.paymentSummary.remainingSeconds =
                0;

              this.errorMessage =
                'Booking hold has expired. Payment cannot be completed.';

              this.stopCountdown();

            } else {

              this.startCountdown();
            }

          } else {

            this.errorMessage =
              response.message ||
              'Unable to load payment summary.';
          }

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to load payment summary.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // COUNTDOWN TIMER
  // =========================================

  startCountdown(): void {

    this.stopCountdown();

    this.countdownTimer =
      setInterval(
        () => {

          if (!this.paymentSummary) {
            return;
          }

          if (
            this.paymentSummary.remainingSeconds > 0
          ) {

            this.paymentSummary.remainingSeconds--;

          } else {

            this.paymentSummary.remainingSeconds =
              0;

            this.paymentSummary.isExpired =
              true;

            this.errorMessage =
              'Booking hold has expired. Payment cannot be completed.';

            this.stopCountdown();
          }

          this.cdr.detectChanges();

        },
        1000
      );
  }

  stopCountdown(): void {

    if (this.countdownTimer) {

      clearInterval(
        this.countdownTimer
      );

      this.countdownTimer =
        null;
    }
  }

  // =========================================
  // TIMER DISPLAY
  // =========================================

  get formattedRemainingTime(): string {

    if (!this.paymentSummary) {
      return '00:00';
    }

    const totalSeconds =
      Math.max(
        0,
        this.paymentSummary.remainingSeconds
      );

    const minutes =
      Math.floor(
        totalSeconds / 60
      );

    const seconds =
      totalSeconds % 60;

    return (
      `${minutes
        .toString()
        .padStart(2, '0')}:` +
      `${seconds
        .toString()
        .padStart(2, '0')}`
    );
  }

  // =========================================
  // CARD NUMBER INPUT
  // =========================================

  onCardNumberInput(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    let value =
      input.value
        .replace(/\D/g, '')
        .substring(0, 16);

    value =
      value.replace(
        /(\d{4})(?=\d)/g,
        '$1 '
      );

    this.paymentForm
      .controls
      .cardNumber
      .setValue(
        value,
        {
          emitEvent: false
        }
      );
  }

  // =========================================
  // EXPIRY INPUT
  // =========================================

  onExpiryInput(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    let value =
      input.value
        .replace(/\D/g, '')
        .substring(0, 4);

    if (
      value.length > 2
    ) {

      value =
        `${value.substring(0, 2)}/${value.substring(2)}`;
    }

    this.paymentForm
      .controls
      .expiryDate
      .setValue(
        value,
        {
          emitEvent: false
        }
      );
  }

  // =========================================
  // CVV INPUT
  // =========================================

  onCvvInput(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    const value =
      input.value
        .replace(/\D/g, '')
        .substring(0, 3);

    this.paymentForm
      .controls
      .cvv
      .setValue(
        value,
        {
          emitEvent: false
        }
      );
  }

  // =========================================
  // FAKE CARD VALIDATORS
  // =========================================

  private cardNumberValidator():
    ValidatorFn {

    return (
      control: AbstractControl
    ): ValidationErrors | null => {

      const value =
        String(
          control.value ?? ''
        )
          .replace(/\s/g, '');

      if (!value) {
        return null;
      }

      if (
        !/^[0-9]{16}$/
          .test(value)
      ) {

        return {
          invalidCardNumber: true
        };
      }

      return null;
    };
  }

  private expiryDateValidator():
    ValidatorFn {

    return (
      control: AbstractControl
    ): ValidationErrors | null => {

      const value =
        String(
          control.value ?? ''
        );

      if (!value) {
        return null;
      }

      if (
        !/^(0[1-9]|1[0-2])\/\d{2}$/
          .test(value)
      ) {

        return {
          invalidExpiry: true
        };
      }

      const [
        monthText,
        yearText
      ] =
        value.split('/');

      const month =
        Number(monthText);

      const year =
        2000 +
        Number(yearText);

      const now =
        new Date();

      const currentMonth =
        now.getMonth() + 1;

      const currentYear =
        now.getFullYear();

      if (
        year < currentYear ||
        (
          year === currentYear &&
          month < currentMonth
        )
      ) {

        return {
          expiredCard: true
        };
      }

      return null;
    };
  }

  // =========================================
  // PAY NOW
  // =========================================

  payNow(): void {

    if (
      this.paymentForm.invalid
    ) {

      this.paymentForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid ATM / Debit Card details.';

      this.cdr.detectChanges();

      return;
    }

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

    this.cdr.detectChanges();

    // IMPORTANT:
    // Fake card details are NOT sent to backend.
    // Existing backend payment logic is used.
    this.paymentService
      .createPayment(
        this.bookingId
      )
      .subscribe({

        next: (response) => {

          this.isPaying = false;

          if (
            response.success &&
            response.data
          ) {

            this.payment =
              response.data;

            this.stopCountdown();

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

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.isPaying = false;

          if (
            error.status === 409
          ) {

            this.errorMessage =
              error?.error?.message ||
              'Payment has already been completed or the booking is no longer available.';

            this.cdr.detectChanges();

            return;
          }

          this.errorMessage =
            error?.error?.message ||
            'Payment could not be completed.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // CAN PAY
  // =========================================

  get canPay(): boolean {

    return !!(
      this.paymentSummary &&
      !this.paymentSummary.isExpired &&
      this.paymentSummary.remainingSeconds > 0 &&
      this.paymentForm.valid &&
      !this.isPaying
    );
  }

  // =========================================
  // BACK
  // =========================================

  goBack(): void {

    this.router.navigate([
      '/bookings',
      this.bookingId,
      'hold'
    ]);
  }

  // =========================================
  // DESTROY
  // =========================================

  ngOnDestroy(): void {

    this.stopCountdown();
  }
}