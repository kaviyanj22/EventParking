import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Booking } from '../../../core/models/booking.model';
import { BookingService } from '../../../core/services/booking.service';
import { BookingStateService } from '../../../core/services/booking-state.service';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking-confirmation.component.html',
  styleUrl: './booking-confirmation.component.css'
})
export class BookingConfirmationComponent implements OnInit {

  bookingId = 0;

  booking: Booking | null = null;

  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: BookingService,
    private bookingStateService: BookingStateService
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

            // Booking flow is finished.
            // Clear temporary seat/parking selection.
            if (
              response.data.status
                .toLowerCase() === 'confirmed'
            ) {
              this.bookingStateService.clearBooking();
            }

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

  goToMyBookings(): void {
    this.router.navigate(['/my-bookings']);
  }

  goToEvents(): void {
    this.router.navigate(['/events']);
  }
}