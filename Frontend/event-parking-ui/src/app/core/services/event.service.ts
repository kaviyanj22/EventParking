import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Event,
  EventCreate,
  EventUpdate,
  EventFilter
} from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {

 private readonly apiUrl =
  `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) {}

  // Get all events + optional search/filter
  getAll(filter?: EventFilter): Observable<Event[]> {

    let params = new HttpParams();

    if (filter?.name) {
      params = params.set('name', filter.name);
    }

    if (filter?.date) {
      params = params.set('date', filter.date);
    }

    if (filter?.venueId !== undefined) {
      params = params.set(
        'venueId',
        filter.venueId.toString()
      );
    }

    if (filter?.categoryId !== undefined) {
      params = params.set(
        'categoryId',
        filter.categoryId.toString()
      );
    }

    return this.http.get<Event[]>(
      this.apiUrl,
      { params }
    );
  }

  // Get one event
  getById(id: number): Observable<Event> {
    return this.http.get<Event>(
      `${this.apiUrl}/${id}`
    );
  }

  // Admin - create event
  create(data: EventCreate): Observable<Event> {
    return this.http.post<Event>(
      this.apiUrl,
      data
    );
  }

  // Admin - update event
  update(
    id: number,
    data: EventUpdate
  ): Observable<Event> {

    return this.http.put<Event>(
      `${this.apiUrl}/${id}`,
      data
    );
  }

  // Admin - delete event
  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}