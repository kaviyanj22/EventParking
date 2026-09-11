import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface BookingSelectionState {
  eventId: number | null;
  seatIds: number[];
  parkingSlotId: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class BookingStateService {

  private readonly initialState: BookingSelectionState = {
    eventId: null,
    seatIds: [],
    parkingSlotId: null
  };

  private bookingStateSubject =
    new BehaviorSubject<BookingSelectionState>(this.initialState);

  bookingState$ = this.bookingStateSubject.asObservable();

  get currentState(): BookingSelectionState {
    return this.bookingStateSubject.value;
  }

  setEvent(eventId: number): void {
    this.bookingStateSubject.next({
      ...this.currentState,
      eventId
    });
  }

  setSeats(seatIds: number[]): void {
    this.bookingStateSubject.next({
      ...this.currentState,
      seatIds
    });
  }

  setParking(parkingSlotId: number | null): void {
    this.bookingStateSubject.next({
      ...this.currentState,
      parkingSlotId
    });
  }

  hasSelectedSeats(): boolean {
    return this.currentState.seatIds.length > 0;
  }

  clearBooking(): void {
    this.bookingStateSubject.next({
      eventId: null,
      seatIds: [],
      parkingSlotId: null
    });
  }
}