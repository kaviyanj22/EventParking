import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  Notification
} from '../../../core/models/notification.model';

import {
  NotificationService
} from '../../../core/services/notification.service';

import {
  AuthService
} from '../../../core/services/auth.service';

@Component({
  selector: 'app-notification-list',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './notification-list.component.html',
  styleUrl: './notification-list.component.css'
})
export class NotificationListComponent implements OnInit {

  customerId = 0;

  notifications: Notification[] = [];

  isLoading = false;

  errorMessage = '';

  constructor(
    private notificationService: NotificationService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {

    const currentUser =
      this.authService.getCurrentUser();

    if (!currentUser) {

      this.router.navigate([
        '/login'
      ]);

      return;
    }

    this.customerId =
      currentUser.customerId;

    this.loadNotifications();
  }

  loadNotifications(): void {

    if (!this.customerId) {

      this.errorMessage =
        'Customer information is not available.';

      return;
    }

    this.isLoading = true;

    this.errorMessage = '';

    this.notificationService
      .getCustomerNotifications(
        this.customerId
      )
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          if (response.success) {

            this.notifications =
              response.data ?? [];

          } else {

            this.notifications = [];

            this.errorMessage =
              response.message ||
              'Unable to load notifications.';
          }

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.isLoading = false;

          this.notifications = [];

          this.errorMessage =
            error?.error?.message ||
            'Unable to load notifications.';

          this.cdr.markForCheck();
        }

      });
  }

  markAsRead(
    notification: Notification
  ): void {

    if (notification.isRead) {
      return;
    }

    this.notificationService
      .markAsRead(
        notification.notificationId
      )
      .subscribe({

        next: (response) => {

          if (
            response.success &&
            response.data
          ) {

            this.notifications =
              this.notifications.map(
                item =>
                  item.notificationId ===
                    notification.notificationId
                    ? response.data!
                    : item
              );

          } else {

            this.errorMessage =
              response.message ||
              'Unable to update notification.';
          }

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ||
            'Unable to update notification.';

          this.cdr.markForCheck();
        }

      });
  }
}
