import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth.service';

import {
  ResetPasswordRequest
} from '../../../core/models/reset-password.model';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {

  isLoading = false;

  errorMessage = '';

  successMessage = '';

  token = '';

  showPassword = false;

  showConfirmPassword = false;

  resetPasswordForm;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {

    this.resetPasswordForm =
      this.formBuilder.group({

        newPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(8)
          ]
        ],

        confirmPassword: [
          '',
          [
            Validators.required
          ]
        ]

      });
  }

  ngOnInit(): void {

    this.token =
      this.route.snapshot.queryParamMap
        .get('token') ?? '';

    if (!this.token) {
      this.errorMessage =
        'Password reset token is missing.';
    }
  }

  get newPassword() {
    return this.resetPasswordForm.controls.newPassword;
  }

  get confirmPassword() {
    return this.resetPasswordForm.controls.confirmPassword;
  }

  togglePassword(): void {
    this.showPassword =
      !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword =
      !this.showConfirmPassword;
  }

  onSubmit(): void {

    this.errorMessage = '';
    this.successMessage = '';

    if (!this.token) {
      this.errorMessage =
        'Password reset token is missing.';
      return;
    }

    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    if (
      this.newPassword.value !==
      this.confirmPassword.value
    ) {
      this.errorMessage =
        'Passwords do not match.';
      return;
    }

    const request: ResetPasswordRequest = {

      token: this.token,

      newPassword:
        this.newPassword.value ?? '',

      confirmPassword:
        this.confirmPassword.value ?? ''

    };

    this.isLoading = true;

    this.authService
      .resetPassword(request)
      .subscribe({

        next: response => {

          this.isLoading = false;

          if (!response.success) {

            this.errorMessage =
              response.message ||
              'Unable to reset password.';

            return;
          }

          this.successMessage =
            response.message ||
            'Password reset successfully.';

          this.resetPasswordForm.reset();

          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        },

        error: error => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to reset your password. The reset link may be invalid or expired.';

        }

      });
  }
}