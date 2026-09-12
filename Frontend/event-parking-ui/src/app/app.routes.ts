import { Routes } from '@angular/router';

import { customerGuard } from './core/guards/customer.guard';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { checkoutGuard } from './core/guards/checkout.guard';

export const routes: Routes = [

  // =========================================
  // MEMBER 1 - AUTH
  // =========================================

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(m => m.Login)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register')
        .then(m => m.Register)
  },

  {
    path: 'verify-email',
    loadComponent: () =>
      import('./features/auth/verify-email/verify-email')
        .then(m => m.VerifyEmail)
  },

  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password')
        .then(m => m.ForgotPassword)
  },

  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/auth/reset-password/reset-password')
        .then(m => m.ResetPassword)
  },

  // =========================================
  // MEMBER 1 - CUSTOMER PROFILE
  // =========================================

  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/customer/profile/profile')
        .then(m => m.Profile)
  },

  // =========================================
  // MEMBER 2 - PUBLIC EVENTS
  // =========================================

  {
    path: 'events',
    loadComponent: () =>
      import(
        './features/events/event-list/event-list.component'
      ).then(m => m.EventListComponent)
  },

  // =========================================
  // MEMBER 3 - CUSTOMER SEAT SELECTION
  // =========================================

  {
    path: 'events/:id/seats',
    canActivate: [customerGuard],
    loadComponent: () =>
      import('./features/seats/seat-map/seat-map')
        .then(m => m.SeatMapComponent)
  },

  // =========================================
  // MEMBER 3 - CUSTOMER PARKING SELECTION
  // =========================================

  {
    path: 'events/:id/parking',
    canActivate: [customerGuard],
    loadComponent: () =>
      import('./features/parking/parking-map/parking-map')
        .then(m => m.ParkingMapComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER CHECKOUT
  // =========================================

  {
    path: 'checkout',
    canActivate: [
      customerGuard,
      checkoutGuard
    ],
    loadComponent: () =>
      import(
        './features/bookings/checkout/checkout.component'
      ).then(m => m.CheckoutComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER BOOKING HOLD
  // =========================================

  {
    path: 'bookings/:id/hold',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/bookings/booking-hold/booking-hold.component'
      ).then(m => m.BookingHoldComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER PAYMENT
  // =========================================

  {
    path: 'bookings/:id/payment',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/payments/payment/payment.component'
      ).then(m => m.PaymentComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER BOOKING CONFIRMATION
  // =========================================

  {
    path: 'bookings/:id/confirmation',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/bookings/booking-confirmation/booking-confirmation.component'
      ).then(m => m.BookingConfirmationComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER BOOKING DETAILS
  // =========================================

  {
    path: 'bookings/:id',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/bookings/booking-details/booking-details.component'
      ).then(m => m.BookingDetailsComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER MY BOOKINGS
  // =========================================

  {
    path: 'my-bookings',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/bookings/my-bookings/my-bookings.component'
      ).then(m => m.MyBookingsComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER PAYMENT HISTORY
  // =========================================

  {
    path: 'payments',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/payments/payment-history/payment-history.component'
      ).then(m => m.PaymentHistoryComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER RECEIPT
  // =========================================

  {
    path: 'payments/:id/receipt',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/payments/receipt/receipt.component'
      ).then(m => m.ReceiptComponent)
  },

  // =========================================
  // MEMBER 4 - CUSTOMER NOTIFICATIONS
  // =========================================

  {
    path: 'notifications',
    canActivate: [customerGuard],
    loadComponent: () =>
      import(
        './features/notifications/notification-list/notification-list.component'
      ).then(m => m.NotificationListComponent)
  },

  // =========================================
  // ADMIN DASHBOARD
  // =========================================

  {
    path: 'admin',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/dashboard/dashboard.component'
      ).then(m => m.AdminDashboardComponent)
  },

  // =========================================
  // ADMIN EVENTS
  // =========================================

  {
    path: 'admin/events',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/events/event-list/event-list.component'
      ).then(m => m.EventListComponent)
  },

  // =========================================
  // ADMIN VENUES
  // =========================================

  {
    path: 'admin/venues',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/venues/venue-list/venue-list.component'
      ).then(m => m.VenueListComponent)
  },

  // =========================================
  // ADMIN CATEGORIES
  // =========================================

  {
    path: 'admin/categories',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/categories/category-list/category-list.component'
      ).then(m => m.CategoryListComponent)
  },

  // =========================================
  // ADMIN SEAT MANAGEMENT
  // =========================================

  {
    path: 'admin/seat-management',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/seat-management/seat-management'
      ).then(m => m.SeatManagementComponent)
  },

  // =========================================
  // ADMIN PARKING MANAGEMENT
  // =========================================

  {
    path: 'admin/parking-management',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/admin/parking-management/parking-management'
      ).then(m => m.ParkingManagementComponent)
  },

  // =========================================
  // ADMIN BOOKINGS
  // =========================================

  {
    path: 'admin/bookings',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/bookings/bookings.component'
      ).then(m => m.AdminBookingsComponent)
  },

  // =========================================
  // ADMIN PAYMENTS
  // =========================================

  {
    path: 'admin/payments',
    canActivate: [
      authGuard,
      adminGuard
    ],
    loadComponent: () =>
      import(
        './features/payments/payments.component'
      ).then(m => m.AdminPaymentsComponent)
  },

  // =========================================
  // DEFAULT
  // =========================================

  {
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  },

  // =========================================
  // FALLBACK - ALWAYS LAST
  // =========================================

  {
    path: '**',
    redirectTo: 'events'
  }

];