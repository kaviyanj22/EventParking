import { Routes } from '@angular/router';
import { ParkingMapComponent } from './features/parking/parking-map/parking-map';

<<<<<<< HEAD
export const routes: Routes = [
  {
    path: '',
    redirectTo: 'parking/1',
    pathMatch: 'full'
  },
  {
    path: 'parking/:id',
    component: ParkingMapComponent
  }
=======
import {
  authGuard
} from './core/guards/auth.guard';

export const routes: Routes = [

  // LOGIN
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(m => m.Login)
  },

  // REGISTER
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register')
        .then(m => m.Register)
  },

  // VERIFY EMAIL
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./features/auth/verify-email/verify-email')
        .then(m => m.VerifyEmail)
  },

  // FORGOT PASSWORD
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password')
        .then(m => m.ForgotPassword)
  },

  // RESET PASSWORD
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/auth/reset-password/reset-password')
        .then(m => m.ResetPassword)
  },

  // CUSTOMER PROFILE - LOGIN REQUIRED
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/customer/profile/profile')
        .then(m => m.Profile)
  },

  // DEFAULT
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  // TEMPORARY FALLBACK
  {
    path: '**',
    redirectTo: 'login'
  }

>>>>>>> origin/Devlop
];