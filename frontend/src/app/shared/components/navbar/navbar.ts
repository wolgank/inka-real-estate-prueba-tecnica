import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, HlmButtonImports],
  template: `
    <nav class="sticky top-0 z-40 w-full border-b border-zinc-200 bg-white/95 backdrop-blur">
      <div class="max-w-6xl mx-auto px-4 h-16 flex items-center justify-between">
        <div class="flex items-center gap-6">
          <a routerLink="/products" class="text-xl font-bold tracking-tight">Inka Real Estate</a>
          
          <div class="hidden md:flex gap-4">
            <a routerLink="/products" 
               routerLinkActive="text-black font-semibold" 
               class="text-sm text-zinc-500 hover:text-black transition-colors">
               Inventario
            </a>
            @if (isAdmin) {
              <a routerLink="/register" 
                routerLinkActive="text-black font-semibold" 
                class="text-sm text-zinc-500 hover:text-black transition-colors">
                Registrar Usuario
              </a>
            }
            </div>
        </div>

        <div class="flex items-center gap-4">
          <span class="text-xs text-zinc-400 hidden sm:inline">Sesión activa: {{ username }}</span>
          <button hlmBtn variant="ghost" size="sm" (click)="onLogout()" class="text-zinc-600">
            Cerrar Sesión
          </button>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  get isAdmin(): boolean {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('role') === 'Admin';
    }
    return false;
  }
  private authService = inject(AuthService);
  private router = inject(Router);

  // Podrías obtener esto de un signal en el AuthService
  username = typeof window !== 'undefined' ? localStorage.getItem('role') : '';

  onLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}