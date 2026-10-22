import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  password = '';

  loading = false;
  error = '';

  login(): void {
    this.error = '';

    if (!this.email || !this.password) {
      this.error = 'Please enter your email and password.';
      return;
    }

    this.loading = true;

    this.authService.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },

      error: error => {
        this.loading = false;

        if (error.status === 401) {
          this.error = 'Invalid email or password.';
        } else {
          this.error = 'Unable to login. Please try again.';
        }
      }
    });
  }

  goToRegister(): void {
    this.router.navigate(['/register']);
  }
}
