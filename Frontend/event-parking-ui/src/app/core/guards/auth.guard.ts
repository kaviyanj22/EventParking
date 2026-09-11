import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (
  route,
  state
) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  // User login ஆகி இருந்தால் page access allow
  if (authService.isLoggedIn()) {
    return true;
  }

  // Login இல்லையென்றால் login pageக்கு அனுப்பு
  return router.createUrlTree(
    ['/login'],
    {
      queryParams: {
        returnUrl: state.url
      }
    }
  );
};