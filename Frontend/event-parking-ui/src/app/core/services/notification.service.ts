import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Notification } from '../models/notification.model';
import { ServiceResult } from './booking.service';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private readonly apiUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  getCustomerNotifications(
    customerId: number
  ): Observable<ServiceResult<Notification[]>> {

    return this.http.get<ServiceResult<Notification[]>>(
      `${this.apiUrl}/customer/${customerId}`
    );
  }

  markAsRead(
    notificationId: number
  ): Observable<ServiceResult<Notification>> {

    return this.http.put<ServiceResult<Notification>>(
      `${this.apiUrl}/${notificationId}/read`,
      {}
    );
  }
}