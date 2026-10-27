
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
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
  submitted = false;

  login(form: NgForm): void {
    if (this.loading) {
      return;
    }

    this.submitted = true;
    this.error = '';

    this.email = this.email.trim();

    if (
      form.invalid ||
      !this.email ||
      !this.password
    ) {
      this.error = 'Please correct the highlighted fields.';
      form.control.markAllAsTouched();
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
