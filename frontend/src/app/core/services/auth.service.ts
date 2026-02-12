import { Injectable, inject, signal, computed, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { LoginRequest, LoginResponse } from '../models/auth.model';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _http = inject(HttpClient);
  private readonly _platformId = inject(PLATFORM_ID);
  private readonly _apiUrl = `${environment.apiUrl}/Auth`;

  private readonly _token = signal<string | null>(this._getInitialToken());
  public readonly isAuthenticated = computed(() => !!this._token());

  private _getInitialToken(): string | null {
    if (isPlatformBrowser(this._platformId)) {
      return localStorage.getItem('token');
    }
    return null;
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this._http.post<LoginResponse>(`${this._apiUrl}/login`, credentials).pipe(
      tap((res) => {
        if (isPlatformBrowser(this._platformId)) {
          localStorage.setItem('token', res.token);
          localStorage.setItem('role', res.role);
        }
        this._token.set(res.token);
      }),
    );
  }
  logout(): void {
    if (isPlatformBrowser(this._platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('role');
    }
    this._token.set(null);
  }
  register(data: any): Observable<any> {
    return this._http.post(`${this._apiUrl}/register`, data);
  }
  private readonly _role = signal<string | null>(
    typeof window !== 'undefined' ? localStorage.getItem('role') : null,
  );

  // Método público para que el guard lo consulte
  getUserRole(): string | null {
    return this._role();
  }
}
