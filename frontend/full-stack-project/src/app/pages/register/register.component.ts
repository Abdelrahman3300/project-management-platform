
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
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
  submitted = false;

  register(form: NgForm): void {
    if (this.loading) {
      return;
    }

    this.submitted = true;
    this.error = '';

    // Normalize user input before validation and submission.
    this.firstName = this.firstName.trim();
    this.lastName = this.lastName.trim();
    this.email = this.email.trim();

    if (
      form.invalid ||
      !this.firstName ||
      !this.lastName ||
      !this.email ||
      !this.password
    ) {
      this.error = 'Please correct the highlighted fields.';
      form.control.markAllAsTouched();
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
          this.error = 'A user with this email already exists.';
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
