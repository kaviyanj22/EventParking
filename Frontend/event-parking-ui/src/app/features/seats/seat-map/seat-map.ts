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

import { forkJoin } from 'rxjs';

import {
  Seat
} from '../../../core/models/seat.model';

import {
  Event as EventModel
} from '../../../core/models/event.model';

import {
  SeatService
} from '../../../core/services/seat.service';

import {
  EventService
} from '../../../core/services/event.service';

import {
  BookingStateService
} from '../../../core/services/booking-state.service';

import {
  SeatComponent
} from '../seat/seat';

@Component({
  selector: 'app-seat-map',
  standalone: true,
  imports: [
    CommonModule,
    SeatComponent
  ],
  templateUrl: './seat-map.html',
  styleUrl: './seat-map.css'
})
export class SeatMapComponent implements OnInit {

  eventId = 0;

  event: EventModel | null = null;

  seats: Seat[] = [];

  selectedSeats: Seat[] = [];

  loading = false;

  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private seatService: SeatService,
    private eventService: EventService,
    private bookingState: BookingStateService,
    private cdr: ChangeDetectorRef
  ) {}

  // =========================================
  // INITIAL LOAD
  // =========================================

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

    const currentState =
      this.bookingState.currentState;

    // If customer opened another event,
    // remove old booking selections.
    if (
      currentState.eventId !==
      this.eventId
    ) {

      this.bookingState
        .setEvent(this.eventId);

      this.bookingState
        .setSeats([]);

      this.bookingState
        .setParking(null);
    }

    this.loadSeatMap();
  }

  // =========================================
  // LOAD EVENT + SEATS
  // =========================================

  loadSeatMap(): void {

    this.loading = true;

    this.errorMessage = '';

    this.cdr.detectChanges();

    forkJoin({

      event:
        this.eventService
          .getById(this.eventId),

      seats:
        this.seatService
          .getSeats(this.eventId)

    }).subscribe({

      next: (result) => {

        this.event =
          result.event;

        /*
          IMPORTANT PRICE LOGIC

          Seat custom price exists
          → use seat price

          Seat custom price is null
          → use Event Default Ticket Price
        */

        this.seats =
          result.seats
            .map((seat) => ({

              ...seat,

              price:
                seat.price ??
                result.event.ticketPrice

            }))
            .sort(
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

        // Restore selected seats
        // if customer comes back to this page.
        const savedSeatIds =
          this.bookingState
            .currentState
            .seatIds;

        this.selectedSeats =
          this.seats.filter(
            seat =>
              savedSeatIds.includes(
                seat.seatId
              ) &&
              seat.status
                .toLowerCase() ===
                'available'
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
  // SELECT / DESELECT SEAT
  // =========================================

  toggleSeat(
    seat: Seat
  ): void {

    if (
      seat.status
        .toLowerCase() !==
      'available'
    ) {
      return;
    }

    const index =
      this.selectedSeats
        .findIndex(
          selectedSeat =>
            selectedSeat.seatId ===
            seat.seatId
        );

    if (index >= 0) {

      this.selectedSeats
        .splice(index, 1);

    } else {

      this.selectedSeats
        .push(seat);
    }

    this.bookingState
      .setSeats(
        this.selectedSeats.map(
          selectedSeat =>
            selectedSeat.seatId
        )
      );

    this.cdr.detectChanges();
  }

  // =========================================
  // CHECK SELECTED
  // =========================================

  isSelected(
    seat: Seat
  ): boolean {

    return this.selectedSeats
      .some(
        selectedSeat =>
          selectedSeat.seatId ===
          seat.seatId
      );
  }

  // =========================================
  // TOTAL PRICE
  // =========================================

  get totalPrice(): number {

    return this.selectedSeats
      .reduce(
        (
          total,
          seat
        ) =>
          total +
          (seat.price ?? 0),
        0
      );
  }

  // =========================================
  // CONTINUE TO PARKING
  // =========================================

  continueBooking(): void {

    if (
      this.selectedSeats.length === 0
    ) {

      this.errorMessage =
        'Please select at least one seat.';

      this.cdr.detectChanges();

      return;
    }

    this.bookingState
      .setEvent(this.eventId);

    this.bookingState
      .setSeats(
        this.selectedSeats.map(
          seat =>
            seat.seatId
        )
      );

    this.router.navigate([
      '/events',
      this.eventId,
      'parking'
    ]);
  }
}