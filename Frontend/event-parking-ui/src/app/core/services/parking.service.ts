import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ParkingSlot,
  ParkingSlotCreate,
  ParkingSlotUpdate,
  ParkingLayoutCreate
} from '../models/parking-slot.model';

@Injectable({
  providedIn: 'root'
})
export class ParkingService {

  private readonly apiUrl = 'https://localhost:7168/api/events';

  constructor(private http: HttpClient) {}

  getParkingSlots(eventId: number): Observable<ParkingSlot[]> {
    return this.http.get<ParkingSlot[]>(
      `${this.apiUrl}/${eventId}/parking-slots`
    );
  }

  getParkingSlot(
    eventId: number,
    slotId: number
  ): Observable<ParkingSlot> {
    return this.http.get<ParkingSlot>(
      `${this.apiUrl}/${eventId}/parking-slots/${slotId}`
    );
  }

  createParkingSlot(
    eventId: number,
    slot: ParkingSlotCreate
  ): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${eventId}/parking-slots/single`,
      slot
    );
  }

  createParkingLayout(
    eventId: number,
    layout: ParkingLayoutCreate
  ): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/${eventId}/parking-slots`,
      layout
    );
  }

  updateParkingSlot(
    eventId: number,
    slotId: number,
    slot: ParkingSlotUpdate
  ): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/${eventId}/parking-slots/${slotId}`,
      slot
    );
  }

  deleteParkingSlot(
    eventId: number,
    slotId: number
  ): Observable<any> {
    return this.http.delete(
      `${this.apiUrl}/${eventId}/parking-slots/${slotId}`
    );
  }
}