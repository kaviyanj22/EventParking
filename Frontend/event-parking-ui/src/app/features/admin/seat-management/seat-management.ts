import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ReactiveFormsModule,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';

import { forkJoin } from 'rxjs';

import {
  Seat,
  SeatCreate,
  SeatUpdate
} from '../../../core/models/seat.model';

import {
  SeatService
} from '../../../core/services/seat.service';

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

  // =========================================
  // SINGLE SEAT FORM
  // =========================================

  seatForm = new FormGroup({

    seatSectionId:
      new FormControl<number | null>(
        null
      ),

    seatNumber:
      new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(20)
        ]
      }),

    rowName:
      new FormControl<string | null>(
        null
      ),

    columnNumber:
      new FormControl<number | null>(
        null
      ),

    positionX:
      new FormControl<number | null>(
        null
      ),

    positionY:
      new FormControl<number | null>(
        null
      ),

    seatType:
      new FormControl<string | null>(
        null
      ),

    // Optional custom seat price.
    // null = use Event Default Ticket Price
    price:
      new FormControl<number | null>(
        null,
        [
          Validators.min(0)
        ]
      ),

    status:
      new FormControl(
        'Available',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.maxLength(20)
          ]
        }
      )

  });

  // =========================================
  // BULK SEAT FORM
  // =========================================

  bulkSeatForm = new FormGroup({

    seatType:
      new FormControl(
        'Normal',
        {
          nonNullable: true,
          validators: [
            Validators.required
          ]
        }
      ),

    rowName:
      new FormControl(
        'A',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.maxLength(10)
          ]
        }
      ),

    startNumber:
      new FormControl(
        1,
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.min(1)
          ]
        }
      ),

    seatCount:
      new FormControl(
        10,
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.min(1),
            Validators.max(500)
          ]
        }
      ),

    // IMPORTANT:
    // null / blank = Event Default Ticket Price
    // value = Custom price for these seats
    price:
      new FormControl<number | null>(
        null,
        [
          Validators.min(0)
        ]
      )

  });

  constructor(
    private seatService: SeatService,
    private cdr: ChangeDetectorRef
  ) {}

  // =========================================
  // LOAD SEATS
  // =========================================

  loadSeats(): void {

    if (
      !this.eventId ||
      this.eventId <= 0
    ) {

      this.errorMessage =
        'Please enter a valid Event ID.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.seatService
      .getSeats(eventId)
      .subscribe({

        next: (seats) => {

          this.seats =
            [...seats].sort(
              (a, b) =>
                a.seatNumber.localeCompare(
                  b.seatNumber,
                  undefined,
                  {
                    numeric: true,
                    sensitivity: 'base'
                  }
                )
            );

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to load seats.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // BULK CREATE SEATS
  // =========================================

  createBulkSeats(): void {

    if (
      !this.eventId ||
      this.eventId <= 0
    ) {

      this.errorMessage =
        'Please enter a valid Event ID.';

      this.cdr.detectChanges();

      return;
    }

    if (
      this.bulkSeatForm.invalid
    ) {

      this.bulkSeatForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid bulk seat details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const formValue =
      this.bulkSeatForm
        .getRawValue();

    const rowName =
      formValue.rowName
        .trim()
        .toUpperCase();

    const seatType =
      formValue.seatType
        .trim();

    const startNumber =
      formValue.startNumber;

    const seatCount =
      formValue.seatCount;

    const customPrice =
      formValue.price;

    const requests = [];

    for (
      let i = 0;
      i < seatCount;
      i++
    ) {

      const number =
        startNumber + i;

      const seat:
        SeatCreate = {

        seatSectionId:
          null,

        seatNumber:
          `${rowName}${number}`,

        rowName:
          rowName,

        columnNumber:
          number,

        positionX:
          number,

        positionY:
          null,

        seatType:
          seatType,

        // null means:
        // use Event Default Ticket Price
        price:
          customPrice

      };

      requests.push(
        this.seatService
          .createSeat(
            eventId,
            seat
          )
      );
    }

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    forkJoin(requests)
      .subscribe({

        next: () => {

          this.loading = false;

          if (
            customPrice === null ||
            customPrice === undefined
          ) {

            this.successMessage =
              `${seatCount} ${seatType} seats created successfully. Default event ticket price will be used.`;

          } else {

            this.successMessage =
              `${seatCount} ${seatType} seats created successfully with custom price LKR ${customPrice}.`;

          }

          this.bulkSeatForm
            .reset({

              seatType: 'Normal',

              rowName: 'A',

              startNumber: 1,

              seatCount: 10,

              price: null

            });

          this.loadSeats();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to create bulk seats.';

          this.loadSeats();

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // CREATE SINGLE SEAT
  // =========================================

  createSeat(): void {

    if (
      !this.eventId ||
      this.eventId <= 0
    ) {

      this.errorMessage =
        'Please enter a valid Event ID.';

      this.cdr.detectChanges();

      return;
    }

    if (
      this.seatForm.invalid
    ) {

      this.seatForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid seat details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const formValue =
      this.seatForm
        .getRawValue();

    const seat:
      SeatCreate = {

      seatSectionId:
        formValue.seatSectionId,

      seatNumber:
        formValue.seatNumber
          .trim()
          .toUpperCase(),

      rowName:
        formValue.rowName
          ?.trim()
          .toUpperCase() ||
        null,

      columnNumber:
        formValue.columnNumber,

      positionX:
        formValue.positionX,

      positionY:
        formValue.positionY,

      seatType:
        formValue.seatType
          ?.trim() ||
        null,

      price:
        formValue.price

    };

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.seatService
      .createSeat(
        eventId,
        seat
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Seat created successfully.';

          this.resetForm();

          this.loadSeats();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to create seat.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // EDIT SEAT
  // =========================================

  editSeat(
    seat: Seat
  ): void {

    this.selectedSeatId =
      seat.seatId;

    this.errorMessage = '';

    this.successMessage = '';

    this.seatForm
      .setValue({

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

    this.cdr.detectChanges();
  }

  // =========================================
  // UPDATE SEAT
  // =========================================

  updateSeat(): void {

    if (
      !this.eventId ||
      !this.selectedSeatId
    ) {

      this.errorMessage =
        'Please select a seat to update.';

      this.cdr.detectChanges();

      return;
    }

    if (
      this.seatForm.invalid
    ) {

      this.seatForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid seat details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const seatId =
      this.selectedSeatId;

    const formValue =
      this.seatForm
        .getRawValue();

    const seat:
      SeatUpdate = {

      seatSectionId:
        formValue.seatSectionId,

      seatNumber:
        formValue.seatNumber
          .trim()
          .toUpperCase(),

      rowName:
        formValue.rowName
          ?.trim()
          .toUpperCase() ||
        null,

      columnNumber:
        formValue.columnNumber,

      positionX:
        formValue.positionX,

      positionY:
        formValue.positionY,

      seatType:
        formValue.seatType
          ?.trim() ||
        null,

      price:
        formValue.price,

      status:
        formValue.status

    };

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.seatService
      .updateSeat(
        eventId,
        seatId,
        seat
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Seat updated successfully.';

          this.resetForm();

          this.loadSeats();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to update seat.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // RESET SINGLE FORM
  // =========================================

  resetForm(): void {

    this.selectedSeatId =
      null;

    this.seatForm
      .reset({

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

    this.cdr.detectChanges();
  }

  // =========================================
  // DELETE SEAT
  // =========================================

  deleteSeat(
    seat: Seat
  ): void {

    if (!this.eventId) {
      return;
    }

    const eventId =
      this.eventId;

    const confirmed =
      confirm(
        `Are you sure you want to delete seat ${seat.seatNumber}?`
      );

    if (!confirmed) {
      return;
    }

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.seatService
      .deleteSeat(
        eventId,
        seat.seatId
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Seat deleted successfully.';

          if (
            this.selectedSeatId ===
            seat.seatId
          ) {

            this.resetForm();
          }

          this.loadSeats();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete seat.';

          this.cdr.detectChanges();
        }

      });
  }
}