import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  firstName = '';
  lastName = '';
  email = '';
  password = '';

  loading = false;
  error = '';

  register(): void {
    this.error = '';

    if (
      !this.firstName ||
      !this.lastName ||
      !this.email ||
      !this.password
    ) {
      this.error = 'Please fill in all fields.';
      return;
    }

    if (this.password.length < 6) {
      this.error =
        'Password must be at least 6 characters.';
      return;
    }

    this.loading = true;

    this.authService.register({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/']);
      },

      error: error => {
        this.loading = false;

        if (error.status === 409) {
          this.error =
            'A user with this email already exists.';
        } else {
          this.error =
            'Unable to create account. Please try again.';
        }
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}