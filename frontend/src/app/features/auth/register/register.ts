import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { HlmCardImports } from '@spartan-ng/helm/card';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    HlmButtonImports, 
    HlmInputImports, 
    HlmLabelImports, 
    HlmCardImports
  ],
  templateUrl: './register.html'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  // Estado del formulario con signals
  form = signal({
    username: '',
    password: '',
    firstName: '',
    lastName: '',
    dni: '',
    phoneNumber: '',
    email: '',
    role: 'Employee' // Valor por defecto según tus requerimientos
  });

  loading = signal(false);
  errorMessage = signal<string | null>(null);

  onSubmit(event: Event) {
    event.preventDefault();
    this.loading.set(true);
    this.errorMessage.set(null);

    // Llamamos al método register del servicio
    this.authService.register(this.form()).subscribe({
      next: () => {
        // Redirigimos al login tras el éxito
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.Message || 'Error al crear la cuenta');
        this.loading.set(false);
      }
    });
  }
}