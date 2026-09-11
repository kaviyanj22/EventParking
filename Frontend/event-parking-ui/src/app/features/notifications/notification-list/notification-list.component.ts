import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Notification } from '../../../core/models/notification.model';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-list.component.html',
  styleUrl: './notification-list.component.css'
})
export class NotificationListComponent implements OnChanges {

  @Input() customerId: number | null = null;

  notifications: Notification[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private notificationService: NotificationService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {

    if (changes['customerId'] && this.customerId) {
      this.loadNotifications();
    }
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
      .getCustomerNotifications(this.customerId)
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
        },

        error: (error) => {

          this.isLoading = false;
          this.notifications = [];

          this.errorMessage =
            error.error?.message ||
            'Unable to load notifications.';
        }
      });
  }

  markAsRead(notification: Notification): void {

    if (notification.isRead) {
      return;
    }

    this.notificationService
      .markAsRead(notification.notificationId)
      .subscribe({

        next: (response) => {

          if (response.success && response.data) {

            const index =
              this.notifications.findIndex(
                item =>
                  item.notificationId ===
                  notification.notificationId
              );

            if (index !== -1) {
              this.notifications[index] =
                response.data;
            }

          } else {

            this.errorMessage =
              response.message ||
              'Unable to update notification.';
          }
        },

        error: (error) => {

          this.errorMessage =
            error.error?.message ||
            'Unable to update notification.';
        }
      });
  }
}