import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Booking } from '../../../core/models/booking.model';
import { BookingService } from '../../../core/services/booking.service';

@Component({
  selector: 'app-booking-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking-details.component.html',
  styleUrl: './booking-details.component.css'
})
export class BookingDetailsComponent implements OnInit {

  bookingId = 0;

  booking: Booking | null = null;

  isLoading = true;
  isCancelling = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {

    this.bookingId =
      Number(this.route.snapshot.paramMap.get('id'));

    if (!this.bookingId) {
      this.isLoading = false;
      this.errorMessage = 'Invalid booking.';
      return;
    }

    this.loadBooking();
  }

  loadBooking(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService
      .getBookingById(this.bookingId)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success && response.data) {

            this.booking = response.data;

          } else {

            this.errorMessage =
              response.message ||
              'Unable to load booking details.';
          }
        },

        error: (error) => {

          this.isLoading = false;

          this.errorMessage =
            error.error?.message ||
            'Unable to load booking details.';
        }
      });
  }

  canCancelBooking(): boolean {

    if (!this.booking) {
      return false;
    }

    const status =
      this.booking.status.toLowerCase();

    return (
      status === 'pending' ||
      status === 'confirmed'
    );
  }

  cancelBooking(): void {

    if (
      !this.booking ||
      !this.canCancelBooking() ||
      this.isCancelling
    ) {
      return;
    }

    const confirmed =
      window.confirm(
        `Are you sure you want to cancel booking ${this.booking.bookingNumber}?`
      );

    if (!confirmed) {
      return;
    }

    this.isCancelling = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.bookingService
      .cancelBooking(this.bookingId)
      .subscribe({

        next: (response) => {

          this.isCancelling = false;

          if (response.success) {

            this.successMessage =
              response.message ||
              'Booking cancelled successfully.';

            this.loadBooking();

          } else {

            this.errorMessage =
              response.message ||
              'Unable to cancel booking.';
          }
        },

        error: (error) => {

          this.isCancelling = false;

          this.errorMessage =
            error.error?.message ||
            'Unable to cancel booking.';
        }
      });
  }

  goBack(): void {
    this.router.navigate(['/my-bookings']);
  }
}