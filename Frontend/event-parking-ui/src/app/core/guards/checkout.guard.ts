import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { BookingStateService } from '../services/booking-state.service';

export const checkoutGuard: CanActivateFn = () => {

  const bookingStateService = inject(BookingStateService);
  const router = inject(Router);

  if (bookingStateService.hasSelectedSeats()) {
    return true;
  }

  return router.createUrlTree(['/events']);
};