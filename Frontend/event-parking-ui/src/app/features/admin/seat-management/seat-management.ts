import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';

import {
  Seat,
  SeatCreate,
  SeatUpdate
} from '../../../core/models/seat.model';

import { SeatService } from '../../../core/services/seat.service';

@Component({
  selector: 'app-seat-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './seat-management.html',
  styleUrl: './seat-management.css'
})
export class SeatManagementComponent {

  eventId: number | null = null;

  seats: Seat[] = [];

  loading = false;
  errorMessage = '';
  successMessage = '';

  selectedSeatId: number | null = null;

  seatForm = new FormGroup({

    seatSectionId:
      new FormControl<number | null>(null),

    seatNumber:
      new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(20)
        ]
      }),

    rowName:
      new FormControl<string | null>(null),

    columnNumber:
      new FormControl<number | null>(null),

    positionX:
      new FormControl<number | null>(null),

    positionY:
      new FormControl<number | null>(null),

    seatType:
      new FormControl<string | null>(null),

    price:
      new FormControl<number | null>(
        null,
        [Validators.min(0)]
      ),

    status:
      new FormControl('Available', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(20)
        ]
      })

  });

  constructor(
    private seatService: SeatService
  ) {}

  // =========================
  // LOAD SEATS
  // =========================

  loadSeats(): void {

    if (!this.eventId || this.eventId <= 0) {
      this.errorMessage =
        'Please enter a valid Event ID.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.seatService
      .getSeats(this.eventId)
      .subscribe({

        next: (seats) => {
          this.seats = seats;
          this.loading = false;
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ??
            'Unable to load seats.';

          this.loading = false;
        }

      });
  }

  // =========================
  // CREATE SEAT
  // =========================

  createSeat(): void {

    if (!this.eventId || this.eventId <= 0) {
      this.errorMessage =
        'Please enter a valid Event ID.';
      return;
    }

    if (this.seatForm.invalid) {
      this.seatForm.markAllAsTouched();

      this.errorMessage =
        'Please enter valid seat details.';

      return;
    }

    const formValue =
      this.seatForm.getRawValue();

    const seat: SeatCreate = {

      seatSectionId:
        formValue.seatSectionId,

      seatNumber:
        formValue.seatNumber,

      rowName:
        formValue.rowName || null,

      columnNumber:
        formValue.columnNumber,

      positionX:
        formValue.positionX,

      positionY:
        formValue.positionY,

      seatType:
        formValue.seatType || null,

      price:
        formValue.price
    };

    this.errorMessage = '';
    this.successMessage = '';

    this.seatService
      .createSeat(
        this.eventId,
        seat
      )
      .subscribe({

        next: () => {
          this.successMessage =
            'Seat created successfully.';

          this.resetForm();
          this.loadSeats();
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ??
            'Unable to create seat.';
        }

      });
  }

  // =========================
  // START EDIT
  // =========================

  editSeat(seat: Seat): void {

    this.selectedSeatId =
      seat.seatId;

    this.errorMessage = '';
    this.successMessage = '';

    this.seatForm.setValue({

      seatSectionId:
        seat.seatSectionId,

      seatNumber:
        seat.seatNumber,

      rowName:
        seat.rowName,

      columnNumber:
        seat.columnNumber,

      positionX:
        seat.positionX,

      positionY:
        seat.positionY,

      seatType:
        seat.seatType,

      price:
        seat.price,

      status:
        seat.status

    });
  }

  // =========================
  // UPDATE SEAT
  // =========================

  updateSeat(): void {

    if (
      !this.eventId ||
      !this.selectedSeatId
    ) {
      this.errorMessage =
        'Please select a seat to update.';
      return;
    }

    if (this.seatForm.invalid) {

      this.seatForm.markAllAsTouched();

      this.errorMessage =
        'Please enter valid seat details.';

      return;
    }

    const formValue =
      this.seatForm.getRawValue();

    const seat: SeatUpdate = {

      seatSectionId:
        formValue.seatSectionId,

      seatNumber:
        formValue.seatNumber,

      rowName:
        formValue.rowName || null,

      columnNumber:
        formValue.columnNumber,

      positionX:
        formValue.positionX,

      positionY:
        formValue.positionY,

      seatType:
        formValue.seatType || null,

      price:
        formValue.price,

      status:
        formValue.status
    };

    this.errorMessage = '';
    this.successMessage = '';

    this.seatService
      .updateSeat(
        this.eventId,
        this.selectedSeatId,
        seat
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Seat updated successfully.';

          this.resetForm();
          this.loadSeats();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to update seat.';
        }

      });
  }

  // =========================
  // CANCEL / RESET FORM
  // =========================

  resetForm(): void {

    this.selectedSeatId = null;

    this.seatForm.reset({

      seatSectionId: null,
      seatNumber: '',
      rowName: null,
      columnNumber: null,
      positionX: null,
      positionY: null,
      seatType: null,
      price: null,
      status: 'Available'

    });
  }

  // =========================
  // DELETE SEAT
  // =========================

  deleteSeat(seat: Seat): void {

    if (!this.eventId) {
      return;
    }

    const confirmed = confirm(
      `Are you sure you want to delete seat ${seat.seatNumber}?`
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.seatService
      .deleteSeat(
        this.eventId,
        seat.seatId
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Seat deleted successfully.';

          if (
            this.selectedSeatId ===
            seat.seatId
          ) {
            this.resetForm();
          }

          this.loadSeats();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete seat.';
        }

      });
  }
}