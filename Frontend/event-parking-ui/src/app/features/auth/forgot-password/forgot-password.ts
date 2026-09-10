import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth.service';

import {
  ForgotPasswordRequest
} from '../../../core/models/forgot-password.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {

  isLoading = false;

  errorMessage = '';

  successMessage = '';

  forgotPasswordForm;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService
  ) {

    this.forgotPasswordForm =
      this.formBuilder.group({

        email: [
          '',
          [
            Validators.required,
            Validators.email
          ]
        ]

      });
  }

  get email() {
    return this.forgotPasswordForm.controls.email;
  }

  onSubmit(): void {

    this.errorMessage = '';
    this.successMessage = '';

    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      return;
    }

    const request: ForgotPasswordRequest = {
      email: this.email.value ?? ''
    };

    this.isLoading = true;

    this.authService
      .forgotPassword(request)
      .subscribe({

        next: response => {

          this.isLoading = false;

          if (!response.success) {

            this.errorMessage =
              response.message ||
              'Unable to process password reset request.';

            return;
          }

          this.successMessage =
            response.message ||
            'Password reset instructions have been sent to your email.';

          this.forgotPasswordForm.reset();
        },

        error: error => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to process your request. Please try again.';

        }

      });
  }
}