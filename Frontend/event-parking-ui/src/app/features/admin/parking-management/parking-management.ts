import {
  ChangeDetectorRef,
  Component
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';

import { forkJoin } from 'rxjs';

import {
  ParkingSlot,
  ParkingSlotCreate,
  ParkingSlotUpdate
} from '../../../core/models/parking-slot.model';

import {
  ParkingService
} from '../../../core/services/parking.service';

@Component({
  selector: 'app-parking-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './parking-management.html',
  styleUrl: './parking-management.css'
})
export class ParkingManagementComponent {

  eventId: number | null = null;

  parkingSlots: ParkingSlot[] = [];

  loading = false;

  errorMessage = '';

  successMessage = '';

  selectedParkingSlotId: number | null = null;

  // =========================================
  // SINGLE PARKING SLOT FORM
  // =========================================

  parkingForm = new FormGroup({

    slotNumber: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(20)
      ]
    }),

    zone: new FormControl<string | null>(
      null,
      [
        Validators.maxLength(50)
      ]
    ),

    fee: new FormControl<number>(0, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(0)
      ]
    }),

    status: new FormControl(
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
  // BULK PARKING FORM
  // =========================================

  bulkParkingForm = new FormGroup({

    zone: new FormControl(
      'A',
      {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(50)
        ]
      }
    ),

    slotPrefix: new FormControl(
      'P',
      {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(10)
        ]
      }
    ),

    startNumber: new FormControl(
      1,
      {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(1)
        ]
      }
    ),

    slotCount: new FormControl(
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

    fee: new FormControl(
      500,
      {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(0)
        ]
      }
    )

  });

  constructor(
    private parkingService: ParkingService,
    private cdr: ChangeDetectorRef
  ) {}

  // =========================================
  // LOAD PARKING SLOTS
  // =========================================

  loadParkingSlots(): void {

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

    this.parkingService
      .getParkingSlots(eventId)
      .subscribe({

        next: (slots) => {

          // Natural sorting:
          // B1, B2, B3 ... B10
          this.parkingSlots =
            [...slots].sort(
              (a, b) =>
                a.slotNumber.localeCompare(
                  b.slotNumber,
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
            'Unable to load parking slots.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // BULK CREATE PARKING SLOTS
  // =========================================

  createBulkParkingSlots(): void {

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
      this.bulkParkingForm.invalid
    ) {

      this.bulkParkingForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid bulk parking details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const formValue =
      this.bulkParkingForm
        .getRawValue();

    const zone =
      formValue.zone
        .trim()
        .toUpperCase();

    const slotPrefix =
      formValue.slotPrefix
        .trim()
        .toUpperCase();

    const startNumber =
      formValue.startNumber;

    const slotCount =
      formValue.slotCount;

    const fee =
      formValue.fee;

    const requests = [];

    for (
      let i = 0;
      i < slotCount;
      i++
    ) {

      const number =
        startNumber + i;

      const parkingSlot:
        ParkingSlotCreate = {

        slotNumber:
          `${slotPrefix}${number}`,

        zone:
          zone,

        fee:
          fee
      };

      requests.push(
        this.parkingService
          .createParkingSlot(
            eventId,
            parkingSlot
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

          this.successMessage =
            `${slotCount} parking slots created successfully.`;

          this.bulkParkingForm
            .reset({

              zone: 'A',

              slotPrefix: 'P',

              startNumber: 1,

              slotCount: 10,

              fee: 500

            });

          this.loadParkingSlots();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to create bulk parking slots.';

          this.loadParkingSlots();

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // CREATE SINGLE PARKING SLOT
  // =========================================

  createParkingSlot(): void {

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
      this.parkingForm.invalid
    ) {

      this.parkingForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid parking slot details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const formValue =
      this.parkingForm
        .getRawValue();

    const parkingSlot:
      ParkingSlotCreate = {

      slotNumber:
        formValue.slotNumber
          .trim()
          .toUpperCase(),

      zone:
        formValue.zone
          ?.trim()
          .toUpperCase() ||
        null,

      fee:
        formValue.fee
    };

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.parkingService
      .createParkingSlot(
        eventId,
        parkingSlot
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Parking slot created successfully.';

          this.resetForm();

          this.loadParkingSlots();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to create parking slot.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // EDIT PARKING SLOT
  // =========================================

  editParkingSlot(
    slot: ParkingSlot
  ): void {

    this.selectedParkingSlotId =
      slot.parkingSlotId;

    this.errorMessage = '';

    this.successMessage = '';

    this.parkingForm
      .setValue({

        slotNumber:
          slot.slotNumber,

        zone:
          slot.zone,

        fee:
          slot.fee,

        status:
          slot.status

      });

    this.cdr.detectChanges();
  }

  // =========================================
  // UPDATE PARKING SLOT
  // =========================================

  updateParkingSlot(): void {

    if (
      !this.eventId ||
      !this.selectedParkingSlotId
    ) {

      this.errorMessage =
        'Please select a parking slot to update.';

      this.cdr.detectChanges();

      return;
    }

    if (
      this.parkingForm.invalid
    ) {

      this.parkingForm
        .markAllAsTouched();

      this.errorMessage =
        'Please enter valid parking slot details.';

      this.cdr.detectChanges();

      return;
    }

    const eventId =
      this.eventId;

    const parkingSlotId =
      this.selectedParkingSlotId;

    const formValue =
      this.parkingForm
        .getRawValue();

    const parkingSlot:
      ParkingSlotUpdate = {

      slotNumber:
        formValue.slotNumber
          .trim()
          .toUpperCase(),

      zone:
        formValue.zone
          ?.trim()
          .toUpperCase() ||
        null,

      fee:
        formValue.fee,

      status:
        formValue.status
    };

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.parkingService
      .updateParkingSlot(
        eventId,
        parkingSlotId,
        parkingSlot
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Parking slot updated successfully.';

          this.resetForm();

          this.loadParkingSlots();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to update parking slot.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // RESET FORM
  // =========================================

  resetForm(): void {

    this.selectedParkingSlotId =
      null;

    this.parkingForm
      .reset({

        slotNumber: '',

        zone: null,

        fee: 0,

        status: 'Available'

      });

    this.cdr.detectChanges();
  }

  // =========================================
  // DELETE PARKING SLOT
  // =========================================

  deleteParkingSlot(
    slot: ParkingSlot
  ): void {

    if (!this.eventId) {
      return;
    }

    const eventId =
      this.eventId;

    const confirmed =
      confirm(
        `Are you sure you want to delete parking slot ${slot.slotNumber}?`
      );

    if (!confirmed) {
      return;
    }

    this.loading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.parkingService
      .deleteParkingSlot(
        eventId,
        slot.parkingSlotId
      )
      .subscribe({

        next: () => {

          this.loading = false;

          this.successMessage =
            'Parking slot deleted successfully.';

          if (
            this.selectedParkingSlotId ===
            slot.parkingSlotId
          ) {

            this.resetForm();
          }

          this.loadParkingSlots();

          this.cdr.detectChanges();
        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete parking slot.';

          this.cdr.detectChanges();
        }

      });
  }
}