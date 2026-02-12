import { Component, inject, PLATFORM_ID, signal, effect } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    @if (showNavbar()) {
      <app-navbar />
    }
    <main class="min-h-screen bg-zinc-50">
      <router-outlet />
    </main>
  `
})
export class App {
  public authService = inject(AuthService);
  private platformId = inject(PLATFORM_ID);
  
  // Usamos un signal local para manejar la visibilidad solo en el cliente
  showNavbar = signal(false);

  constructor() {
    // Este efecto se ejecutará cuando cambie el estado de autenticación
    effect(() => {
      if (isPlatformBrowser(this.platformId)) {
        this.showNavbar.set(this.authService.isAuthenticated());
      }
    });
  }
}