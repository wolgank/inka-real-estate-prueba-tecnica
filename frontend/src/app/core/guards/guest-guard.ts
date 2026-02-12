import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

export const guestGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  if (!isPlatformBrowser(platformId)) return true;

  const token = localStorage.getItem('token');

  // Si YA hay token, lo mandamos a products
  if (token) {
    return router.parseUrl('/products');
  }

  // Si no hay token, lo dejamos entrar al login tranquilamente
  return true;
};