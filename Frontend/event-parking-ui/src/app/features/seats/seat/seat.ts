import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Seat } from '../../../core/models/seat.model';

@Component({
  selector: 'app-seat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './seat.html',
  styleUrl: './seat.css'
})
export class SeatComponent {

  @Input({ required: true }) seat!: Seat;
  @Input() selected = false;

  @Output() seatSelected = new EventEmitter<Seat>();

  selectSeat(): void {
    if (this.seat.status.toLowerCase() === 'booked') {
      return;
    }

    this.seatSelected.emit(this.seat);
  }
}