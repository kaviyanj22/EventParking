import {
  Component,
  Input,
  OnChanges,
  SimpleChanges
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { Booking } from '../../../core/models/booking.model';
import { BookingService } from '../../../core/services/booking.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnChanges {

  @Input() customerId: number | null = null;

  bookings: Booking[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnChanges(changes: SimpleChanges): void {

    if (
      changes['customerId'] &&
      this.customerId
    ) {
      this.loadBookings();
    }
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
      .getCustomerBookings(this.customerId)
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
        },

        error: (error) => {

          this.isLoading = false;
          this.bookings = [];

          this.errorMessage =
            error.error?.message ||
            'Unable to load bookings.';
        }
      });
  }

  viewBooking(bookingId: number): void {

    this.router.navigate([
      '/bookings',
      bookingId
    ]);
  }

  getStatusClass(status: string): string {

    return status
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
}