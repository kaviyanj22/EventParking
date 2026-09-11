import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Seat,
  SeatCreate,
  SeatUpdate,
  SeatMapCreate
} from '../models/seat.model';

@Injectable({
  providedIn: 'root'
})
export class SeatService {

  private readonly apiUrl = 'https://localhost:5001/api/events';

  constructor(private http: HttpClient) {}

  getSeats(eventId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(
      `${this.apiUrl}/${eventId}/seats`
    );
  }

  getSeat(eventId: number, seatId: number): Observable<Seat> {
    return this.http.get<Seat>(
      `${this.apiUrl}/${eventId}/seats/${seatId}`
    );
  }

  createSeat(
    eventId: number,
    seat: SeatCreate
  ): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${eventId}/seats/single`,
      seat
    );
  }

  createSeatMap(
    eventId: number,
    seatMap: SeatMapCreate
  ): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${eventId}/seats`,
      seatMap
    );
  }

  updateSeat(
    eventId: number,
    seatId: number,
    seat: SeatUpdate
  ): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/${eventId}/seats/${seatId}`,
      seat
    );
  }

  deleteSeat(
    eventId: number,
    seatId: number
  ): Observable<any> {
    return this.http.delete(
      `${this.apiUrl}/${eventId}/seats/${seatId}`
    );
  }
}