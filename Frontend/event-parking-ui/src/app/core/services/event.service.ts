import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams
} from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  Event,
  EventCreate,
  EventUpdate,
  EventFilter
} from '../models/event.model';

export interface EventImageUploadResponse {
  message: string;
  imageUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {

  private readonly apiUrl =
    `${environment.apiUrl}/events`;

  constructor(
    private http: HttpClient
  ) {}

  // =========================================
  // GET ALL EVENTS
  // =========================================

  getAll(
    filter?: EventFilter
  ): Observable<Event[]> {

    let params =
      new HttpParams();

    if (filter?.name) {
      params =
        params.set(
          'name',
          filter.name
        );
    }

    if (filter?.date) {
      params =
        params.set(
          'date',
          filter.date
        );
    }

    if (
      filter?.venueId !== undefined
    ) {
      params =
        params.set(
          'venueId',
          filter.venueId.toString()
        );
    }

    if (
      filter?.categoryId !== undefined
    ) {
      params =
        params.set(
          'categoryId',
          filter.categoryId.toString()
        );
    }

    return this.http.get<Event[]>(
      this.apiUrl,
      {
        params
      }
    );
  }

  // =========================================
  // GET EVENT BY ID
  // =========================================

  getById(
    id: number
  ): Observable<Event> {

    return this.http.get<Event>(
      `${this.apiUrl}/${id}`
    );
  }

  // =========================================
  // ADMIN - UPLOAD EVENT POSTER IMAGE
  // =========================================

  uploadEventImage(
    file: File
  ): Observable<EventImageUploadResponse> {

    const formData =
      new FormData();

    formData.append(
      'file',
      file
    );

    return this.http.post<EventImageUploadResponse>(
      `${this.apiUrl}/upload-image`,
      formData
    );
  }

  // =========================================
  // ADMIN - CREATE EVENT
  // =========================================

  create(
    data: EventCreate
  ): Observable<Event> {

    return this.http.post<Event>(
      this.apiUrl,
      data
    );
  }

  // =========================================
  // ADMIN - UPDATE EVENT
  // =========================================

  update(
    id: number,
    data: EventUpdate
  ): Observable<Event> {

    return this.http.put<Event>(
      `${this.apiUrl}/${id}`,
      data
    );
  }

  // =========================================
  // ADMIN - DELETE EVENT
  // =========================================

  delete(
    id: number
  ): Observable<void> {

    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}