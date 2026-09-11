export interface Notification {
  notificationId: number;
  customerId: number;

  bookingId?: number | null;
  eventId?: number | null;

  type: string;
  title: string;
  message: string;

  isRead: boolean;

  createdAt: string;
  readAt?: string | null;
}

export interface NotificationRead {
  isRead: boolean;
}