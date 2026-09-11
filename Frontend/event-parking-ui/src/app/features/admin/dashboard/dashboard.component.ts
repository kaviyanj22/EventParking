import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  AdminDashboard,
  DashboardService
} from '../../../core/services/dashboard.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit {

  dashboard: AdminDashboard | null = null;

  isLoading = true;
  errorMessage = '';

  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService
      .getAdminDashboard()
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success && response.data) {
            this.dashboard = response.data;
          } else {
            this.dashboard = null;

            this.errorMessage =
              response.message ||
              'Unable to load admin dashboard.';
          }
        },

        error: (error) => {

          this.isLoading = false;
          this.dashboard = null;

          this.errorMessage =
            error.error?.message ||
            'Unable to load admin dashboard.';
        }
      });
  }

  goToBookings(): void {
    this.router.navigate(['/admin/bookings']);
  }

  goToPayments(): void {
    this.router.navigate(['/admin/payments']);
  }
}