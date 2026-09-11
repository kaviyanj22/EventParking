import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { Seat } from '../../../core/models/seat.model';
import { SeatService } from '../../../core/services/seat.service';
import { SeatComponent } from '../seat/seat';

@Component({
  selector: 'app-seat-map',
  standalone: true,
  imports: [CommonModule, SeatComponent],
  templateUrl: './seat-map.html',
  styleUrl: './seat-map.css'
})
export class SeatMapComponent implements OnInit {

  eventId = 0;

  seats: Seat[] = [];
  selectedSeats: Seat[] = [];

  loading = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private seatService: SeatService
  ) {}

  ngOnInit(): void {
    this.eventId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!this.eventId) {
      this.errorMessage = 'Invalid event.';
      return;
    }

    this.loadSeats();
  }

  loadSeats(): void {
    this.loading = true;
    this.errorMessage = '';

    this.seatService.getSeats(this.eventId).subscribe({
      next: (seats) => {
        this.seats = seats;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load seats.';
        this.loading = false;
      }
    });
  }

  toggleSeat(seat: Seat): void {
    const index = this.selectedSeats.findIndex(
      selectedSeat => selectedSeat.seatId === seat.seatId
    );

    if (index >= 0) {
      this.selectedSeats.splice(index, 1);
    } else {
      this.selectedSeats.push(seat);
    }
  }

  isSelected(seat: Seat): boolean {
    return this.selectedSeats.some(
      selectedSeat => selectedSeat.seatId === seat.seatId
    );
  }

  get totalPrice(): number {
    return this.selectedSeats.reduce(
      (total, seat) => total + (seat.price ?? 0),
      0
    );
  }
}