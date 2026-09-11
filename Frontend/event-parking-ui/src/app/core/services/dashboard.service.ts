import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ServiceResult } from './booking.service';

export interface AdminDashboard {
  totalEvents: number;
  totalBookings: number;
  availableSeats: number;
  occupiedParkingSlots: number;
  totalRevenueCollected: number;
  totalCustomers: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

private readonly apiUrl = `${environment.apiUrl}/admin/dashboard`;

  constructor(private http: HttpClient) {}

  getAdminDashboard():
    Observable<ServiceResult<AdminDashboard>> {

    return this.http.get<ServiceResult<AdminDashboard>>(
      this.apiUrl
    );
  }
}