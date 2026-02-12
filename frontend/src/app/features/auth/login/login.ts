import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginSchema, LoginRequest } from '../../../core/models/auth.model';

// CORRECCIÓN: Importaciones usando el alias que configuró tu CLI (@spartan-ng/helm)
import { HlmButtonImports } from '@spartan-ng/helm/button';

import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { HlmCardImports } from '@spartan-ng/helm/card';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule, 
    HlmButtonImports, 
    HlmInputImports, 
    HlmLabelImports, 
    HlmCardImports,
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  form = signal<LoginRequest>({ username: '', password: '' });
  errorMessage = signal<string | null>(null);
  loading = signal(false);

  onSubmit(event: Event) {
    event.preventDefault();
    this.errorMessage.set(null);

    const result = LoginSchema.safeParse(this.form());
    
    if (!result.success) {
      this.errorMessage.set(result.error.issues[0].message);
      return;
    }

    this.loading.set(true);
    this.authService.login(result.data).subscribe({
      next: () => this.router.navigate(['/products']),
      error: (err) => {
        this.errorMessage.set(err.error?.Message || 'Credenciales inválidas');
        this.loading.set(false);
      }
    });
  }
}