import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  ParkingSlot
} from '../../../core/models/parking-slot.model';

import {
  ParkingService
} from '../../../core/services/parking.service';

import {
  BookingStateService
} from '../../../core/services/booking-state.service';

import {
  ParkingSlotComponent
} from '../parking-slot/parking-slot';

@Component({
  selector: 'app-parking-map',
  standalone: true,
  imports: [
    CommonModule,
    ParkingSlotComponent
  ],
  templateUrl: './parking-map.html',
  styleUrl: './parking-map.css'
})
export class ParkingMapComponent implements OnInit {

  eventId = 0;

  parkingSlots: ParkingSlot[] = [];

  selectedSlot: ParkingSlot | null = null;

  loading = false;

  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parkingService: ParkingService,
    private bookingStateService: BookingStateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    this.eventId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!this.eventId) {

      this.errorMessage =
        'Invalid event.';

      this.cdr.detectChanges();

      return;
    }

    this.bookingStateService
      .setEvent(this.eventId);

    this.loadParkingSlots();
  }

  loadParkingSlots(): void {

    this.loading = true;

    this.errorMessage = '';

    this.parkingService
      .getParkingSlots(this.eventId)
      .subscribe({

        next: (slots) => {

          this.parkingSlots = slots;

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: () => {

          this.errorMessage =
            'Unable to load parking slots.';

          this.loading = false;

          this.cdr.detectChanges();
        }

      });
  }

  selectSlot(
    slot: ParkingSlot
  ): void {

    if (
      this.selectedSlot
        ?.parkingSlotId ===
      slot.parkingSlotId
    ) {

      this.selectedSlot = null;

      this.bookingStateService
        .setParking(null);

      this.cdr.detectChanges();

      return;
    }

    this.selectedSlot = slot;

    this.bookingStateService
      .setParking(
        slot.parkingSlotId
      );

    this.cdr.detectChanges();
  }

  isSelected(
    slot: ParkingSlot
  ): boolean {

    return (
      this.selectedSlot
        ?.parkingSlotId ===
      slot.parkingSlotId
    );
  }

  continueToCheckout(): void {

    if (!this.selectedSlot) {

      this.errorMessage =
        'Please select a parking slot.';

      this.cdr.detectChanges();

      return;
    }

    this.bookingStateService
      .setParking(
        this.selectedSlot.parkingSlotId
      );

    this.router.navigate(
      ['/checkout']
    );
  }

  skipParking(): void {

    this.bookingStateService
      .setParking(null);

    this.router.navigate(
      ['/checkout']
    );
  }
}