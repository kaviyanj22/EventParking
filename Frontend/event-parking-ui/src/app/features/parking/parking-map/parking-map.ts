import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { ParkingSlot } from '../../../core/models/parking-slot.model';
import { ParkingService } from '../../../core/services/parking.service';
import { ParkingSlotComponent } from '../parking-slot/parking-slot';

@Component({
  selector: 'app-parking-map',
  standalone: true,
  imports: [CommonModule, ParkingSlotComponent],
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
    private parkingService: ParkingService
  ) {}

  ngOnInit(): void {
    this.eventId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!this.eventId) {
      this.errorMessage = 'Invalid event.';
      return;
    }

    this.loadParkingSlots();
  }

  loadParkingSlots(): void {
    this.loading = true;
    this.errorMessage = '';

    this.parkingService.getParkingSlots(this.eventId).subscribe({
      next: (slots) => {
        this.parkingSlots = slots;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load parking slots.';
        this.loading = false;
      }
    });
  }

  selectSlot(slot: ParkingSlot): void {
    if (this.selectedSlot?.parkingSlotId === slot.parkingSlotId) {
      this.selectedSlot = null;
      return;
    }

    this.selectedSlot = slot;
  }

  isSelected(slot: ParkingSlot): boolean {
    return this.selectedSlot?.parkingSlotId === slot.parkingSlotId;
  }
}