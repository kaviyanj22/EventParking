import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  BookingStateService,
  BookingSelectionState
} from '../../../core/services/booking-state.service';

import {
  BookingService
} from '../../../core/services/booking.service';

import {
  BookingCreate
} from '../../../core/models/booking.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit {

  bookingState!: BookingSelectionState;

  isLoading = false;
  errorMessage = '';

  constructor(
    private bookingStateService: BookingStateService,
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.bookingState =
      this.bookingStateService.currentState;

    if (
      !this.bookingState.eventId ||
      this.bookingState.seatIds.length === 0
    ) {
      this.router.navigate(['/events']);
    }
  }

  createBooking(): void {

    if (
      !this.bookingState.eventId ||
      this.bookingState.seatIds.length === 0
    ) {
      this.errorMessage =
        'Please select at least one seat.';
      return;
    }

    const request: BookingCreate = {
      eventId: this.bookingState.eventId,
      seatIds: this.bookingState.seatIds,
      parkingSlotId:
        this.bookingState.parkingSlotId
    };

    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService
      .createBooking(request)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (
            response.success &&
            response.data
          ) {

            const bookingId =
              response.data.bookingId;

            this.router.navigate([
              '/bookings',
              bookingId,
              'hold'
            ]);

          } else {

            this.errorMessage =
              response.message ||
              'Unable to create booking.';
          }
        },

        error: (error) => {

          this.isLoading = false;

          if (error.status === 409) {
            this.errorMessage =
              error.error?.message ||
              'One or more selected seats or the parking slot are no longer available.';
            return;
          }

          this.errorMessage =
            error.error?.message ||
            'Something went wrong while creating the booking.';
        }
      });
  }
}