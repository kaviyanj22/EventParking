import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {

  const authService = inject(AuthService);
  const router = inject(Router);

  // Login இல்லையென்றால் login page
  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  // Admin role என்றால் access allow
  if (authService.isAdmin()) {
    return true;
  }

  // Customer admin page access பண்ணினால்
  // events pageக்கு redirect
  return router.createUrlTree(['/events']);
};