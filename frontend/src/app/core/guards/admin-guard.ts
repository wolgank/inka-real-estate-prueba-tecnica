import { inject, PLATFORM_ID } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const platformId = inject(PLATFORM_ID);

  console.log('--- DIAGNÓSTICO GUARD ---');
  console.log('1. ¿Es Navegador?:', isPlatformBrowser(platformId));

  // 1. Si es el servidor (SSR), dejamos pasar para que el cliente decida al hidratar
  if (!isPlatformBrowser(platformId)) {
    console.log('2. Ejecutando en Servidor, permitiendo paso inicial...');
    return true;
  }

  // 2. Usamos el rol que el servicio ya tiene cargado
  const role = authService.getUserRole();
  const token = localStorage.getItem('token');

  console.log('3. Token en LocalStorage:', token ? 'Existe' : 'NULO');
  console.log('4. Rol en LocalStorage:', role);
  console.log('5. Comparación (role === "Admin"):', role === 'Admin');

  if (role === 'Admin') {
    console.log('✅ Acceso concedido a Register');
    return true;
  }

  // 3. Si no es admin, imprimimos un log para depurar y redirigimos
  console.warn('Acceso denegado: El rol actual es:', role);
  return router.parseUrl('/products');
};