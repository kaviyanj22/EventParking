import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Venue,
  VenueCreate,
  VenueUpdate
} from '../models/venue.model';

@Injectable({
  providedIn: 'root'
})
export class VenueService {

  private readonly apiUrl =
  `${environment.apiUrl}/venues`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Venue[]> {
    return this.http.get<Venue[]>(this.apiUrl);
  }

  getById(id: number): Observable<Venue> {
    return this.http.get<Venue>(`${this.apiUrl}/${id}`);
  }

  create(data: VenueCreate): Observable<Venue> {
    return this.http.post<Venue>(this.apiUrl, data);
  }

  update(id: number, data: VenueUpdate): Observable<Venue> {
    return this.http.put<Venue>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAvailable(
    date: string,
    startTime: string,
    endTime: string
  ): Observable<Venue[]> {

    const params = new HttpParams()
      .set('date', date)
      .set('startTime', startTime)
      .set('endTime', endTime);

    return this.http.get<Venue[]>(
      `${this.apiUrl}/available`,
      { params }
    );
  }
}