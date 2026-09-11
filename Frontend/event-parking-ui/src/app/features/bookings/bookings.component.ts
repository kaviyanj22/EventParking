import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { Booking } from '../../core/models/booking.model';
import { BookingService } from '../../core/services/booking.service';

@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css'
})
export class AdminBookingsComponent implements OnInit {

  bookings: Booking[] = [];

  eventId: number | null = null;

  isLoading = false;
  errorMessage = '';

  constructor(
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {

    this.isLoading = true;
    this.errorMessage = '';

    const filterEventId =
      this.eventId && this.eventId > 0
        ? this.eventId
        : undefined;

    this.bookingService
      .getAllBookings(filterEventId)
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success) {
            this.bookings = response.data ?? [];
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

  applyFilter(): void {
    this.loadBookings();
  }

  clearFilter(): void {
    this.eventId = null;
    this.loadBookings();
  }

  viewBooking(bookingId: number): void {
    this.router.navigate(['/bookings', bookingId]);
  }

  getStatusClass(status: string): string {
    return status
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
} 