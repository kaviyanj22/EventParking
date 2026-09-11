import {
  Component,
  OnDestroy,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import {
  BookingHoldStatus,
  BookingService
} from '../../../core/services/booking.service';

@Component({
  selector: 'app-booking-hold',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking-hold.component.html',
  styleUrl: './booking-hold.component.css'
})
export class BookingHoldComponent implements OnInit, OnDestroy {

  bookingId = 0;

  holdStatus: BookingHoldStatus | null = null;

  remainingSeconds = 0;

  isLoading = true;
  errorMessage = '';

  private countdownInterval: ReturnType<typeof setInterval> | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {

    this.bookingId =
      Number(this.route.snapshot.paramMap.get('id'));

    if (!this.bookingId) {
      this.errorMessage = 'Invalid booking.';
      this.isLoading = false;
      return;
    }

    this.loadHoldStatus();
  }

  loadHoldStatus(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService
      .getHoldStatus(this.bookingId)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success && response.data) {

            this.holdStatus = response.data;

            this.remainingSeconds =
              response.data.remainingSeconds;

            if (
              response.data.isExpired ||
              this.remainingSeconds <= 0
            ) {
              this.remainingSeconds = 0;
              this.stopCountdown();
              return;
            }

            this.startCountdown();

          } else {

            this.errorMessage =
              response.message ||
              'Unable to load booking hold status.';
          }
        },

        error: (error) => {

          this.isLoading = false;

          this.errorMessage =
            error.error?.message ||
            'Unable to load booking hold status.';
        }
      });
  }

  startCountdown(): void {

    this.stopCountdown();

    this.countdownInterval = setInterval(() => {

      if (this.remainingSeconds > 0) {

        this.remainingSeconds--;

      } else {

        this.remainingSeconds = 0;

        this.stopCountdown();

        if (this.holdStatus) {
          this.holdStatus.isExpired = true;
          this.holdStatus.status = 'Expired';
        }
      }

    }, 1000);
  }

  get formattedTime(): string {

    const minutes =
      Math.floor(this.remainingSeconds / 60);

    const seconds =
      this.remainingSeconds % 60;

    return `${minutes.toString().padStart(2, '0')}:${seconds
      .toString()
      .padStart(2, '0')}`;
  }

  proceedToPayment(): void {

    if (
      !this.holdStatus ||
      this.holdStatus.isExpired ||
      this.remainingSeconds <= 0
    ) {
      this.errorMessage =
        'Booking hold has expired. Payment cannot be continued.';
      return;
    }

    this.router.navigate([
      '/payments',
      this.bookingId
    ]);
  }

  ngOnDestroy(): void {
    this.stopCountdown();
  }

  private stopCountdown(): void {

    if (this.countdownInterval !== null) {

      clearInterval(this.countdownInterval);

      this.countdownInterval = null;
    }
  }
}