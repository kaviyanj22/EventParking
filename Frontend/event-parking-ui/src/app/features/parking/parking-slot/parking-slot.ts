import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ParkingSlot } from '../../../core/models/parking-slot.model';

@Component({
  selector: 'app-parking-slot',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './parking-slot.html',
  styleUrl: './parking-slot.css'
})
export class ParkingSlotComponent {

  @Input({ required: true }) slot!: ParkingSlot;

  @Input() selected = false;

  @Output() slotSelected = new EventEmitter<ParkingSlot>();

  selectSlot(): void {
    if (this.slot.status.toLowerCase() !== 'available') {
      return;
    }

    this.slotSelected.emit(this.slot);
  }
}