import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  Booking
} from '../../../core/models/booking.model';

import {
  BookingService
} from '../../../core/services/booking.service';

import {
  AuthService
} from '../../../core/services/auth.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {

  customerId = 0;

  bookings: Booking[] = [];

  isLoading = false;

  errorMessage = '';

  constructor(
    private bookingService: BookingService,
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

    this.loadBookings();
  }

  loadBookings(): void {

    if (!this.customerId) {

      this.errorMessage =
        'Customer information is not available.';

      return;
    }

    this.isLoading = true;

    this.errorMessage = '';

    this.bookingService
      .getCustomerBookings(
        this.customerId
      )
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success) {

            this.bookings =
              response.data ?? [];

          } else {

            this.bookings = [];

            this.errorMessage =
              response.message ||
              'Unable to load bookings.';
          }

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.isLoading = false;

          this.bookings = [];

          this.errorMessage =
            error?.error?.message ||
            'Unable to load bookings.';

          this.cdr.markForCheck();
        }

      });
  }

  viewBooking(
    bookingId: number
  ): void {

    this.router.navigate([
      '/bookings',
      bookingId
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
