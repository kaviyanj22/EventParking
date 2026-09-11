import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';

import {
  ParkingSlot,
  ParkingSlotCreate,
  ParkingSlotUpdate
} from '../../../core/models/parking-slot.model';

import { ParkingService } from '../../../core/services/parking.service';

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

  parkingForm = new FormGroup({

    slotNumber: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(20)
      ]
    }),

    zone: new FormControl<string | null>(null, [
      Validators.maxLength(50)
    ]),

    fee: new FormControl<number>(0, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(0)
      ]
    }),

    status: new FormControl('Available', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(20)
      ]
    })

  });

  constructor(
    private parkingService: ParkingService
  ) {}

  // =========================
  // LOAD PARKING SLOTS
  // =========================

  loadParkingSlots(): void {

    if (!this.eventId || this.eventId <= 0) {

      this.errorMessage =
        'Please enter a valid Event ID.';

      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.parkingService
      .getParkingSlots(this.eventId)
      .subscribe({

        next: (slots) => {

          this.parkingSlots = slots;

          this.loading = false;
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to load parking slots.';

          this.loading = false;
        }

      });
  }

  // =========================
  // CREATE PARKING SLOT
  // =========================

  createParkingSlot(): void {

    if (!this.eventId || this.eventId <= 0) {

      this.errorMessage =
        'Please enter a valid Event ID.';

      return;
    }

    if (this.parkingForm.invalid) {

      this.parkingForm.markAllAsTouched();

      this.errorMessage =
        'Please enter valid parking slot details.';

      return;
    }

    const formValue =
      this.parkingForm.getRawValue();

    const parkingSlot: ParkingSlotCreate = {

      slotNumber:
        formValue.slotNumber,

      zone:
        formValue.zone || null,

      fee:
        formValue.fee

    };

    this.errorMessage = '';
    this.successMessage = '';

    this.parkingService
      .createParkingSlot(
        this.eventId,
        parkingSlot
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Parking slot created successfully.';

          this.resetForm();

          this.loadParkingSlots();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to create parking slot.';
        }

      });
  }

  // =========================
  // EDIT PARKING SLOT
  // =========================

  editParkingSlot(slot: ParkingSlot): void {

    this.selectedParkingSlotId =
      slot.parkingSlotId;

    this.errorMessage = '';
    this.successMessage = '';

    this.parkingForm.setValue({

      slotNumber:
        slot.slotNumber,

      zone:
        slot.zone,

      fee:
        slot.fee,

      status:
        slot.status

    });
  }

  // =========================
  // UPDATE PARKING SLOT
  // =========================

  updateParkingSlot(): void {

    if (
      !this.eventId ||
      !this.selectedParkingSlotId
    ) {

      this.errorMessage =
        'Please select a parking slot to update.';

      return;
    }

    if (this.parkingForm.invalid) {

      this.parkingForm.markAllAsTouched();

      this.errorMessage =
        'Please enter valid parking slot details.';

      return;
    }

    const formValue =
      this.parkingForm.getRawValue();

    const parkingSlot: ParkingSlotUpdate = {

      slotNumber:
        formValue.slotNumber,

      zone:
        formValue.zone || null,

      fee:
        formValue.fee,

      status:
        formValue.status

    };

    this.errorMessage = '';
    this.successMessage = '';

    this.parkingService
      .updateParkingSlot(
        this.eventId,
        this.selectedParkingSlotId,
        parkingSlot
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Parking slot updated successfully.';

          this.resetForm();

          this.loadParkingSlots();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to update parking slot.';
        }

      });
  }

  // =========================
  // RESET / CANCEL
  // =========================

  resetForm(): void {

    this.selectedParkingSlotId = null;

    this.parkingForm.reset({

      slotNumber: '',

      zone: null,

      fee: 0,

      status: 'Available'

    });
  }

  // =========================
  // DELETE PARKING SLOT
  // =========================

  deleteParkingSlot(slot: ParkingSlot): void {

    if (!this.eventId) {
      return;
    }

    const confirmed = confirm(
      `Are you sure you want to delete parking slot ${slot.slotNumber}?`
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.parkingService
      .deleteParkingSlot(
        this.eventId,
        slot.parkingSlotId
      )
      .subscribe({

        next: () => {

          this.successMessage =
            'Parking slot deleted successfully.';

          if (
            this.selectedParkingSlotId ===
            slot.parkingSlotId
          ) {
            this.resetForm();
          }

          this.loadParkingSlots();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Unable to delete parking slot.';
        }

      });
  }
}