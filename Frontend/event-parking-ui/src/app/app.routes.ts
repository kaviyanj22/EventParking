import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [

  // MEMBER 1 - AUTH
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

  // MEMBER 1 - CUSTOMER PROFILE
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/customer/profile/profile')
        .then(m => m.Profile)
  },

  // MEMBER 3 - SEATS
  {
    path: 'events/:id/seats',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/seats/seat-map/seat-map')
        .then(m => m.SeatMapComponent)
  },

  // MEMBER 3 - PARKING
  {
    path: 'events/:id/parking',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/parking/parking-map/parking-map')
        .then(m => m.ParkingMapComponent)
  },

  // MEMBER 3 - ADMIN SEAT MANAGEMENT
  {
    path: 'admin/seat-management',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/admin/seat-management/seat-management')
        .then(m => m.SeatManagementComponent)
  },

  // MEMBER 3 - ADMIN PARKING MANAGEMENT
  {
    path: 'admin/parking-management',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/admin/parking-management/parking-management')
        .then(m => m.ParkingManagementComponent)
  },

  // DEFAULT
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  // FALLBACK - ALWAYS LAST
  {
    path: '**',
    redirectTo: 'login'
  }

];
