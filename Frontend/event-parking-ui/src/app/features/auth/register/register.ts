import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  Router,
  RouterLink
} from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth.service';

import {
  RegisterRequest
} from '../../../core/models/register-request.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  isLoading = false;

  errorMessage = '';

  successMessage = '';

  showPassword = false;

  showConfirmPassword = false;

  registerForm;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {

    this.registerForm =
      this.formBuilder.group({

        fullName: [
          '',
          [
            Validators.required,
            Validators.minLength(2),
            Validators.maxLength(100)
          ]
        ],

        email: [
          '',
          [
            Validators.required,
            Validators.email
          ]
        ],

        phone: [
          '',
          [
            Validators.required
          ]
        ],

        password: [
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

  get fullName() {
    return this.registerForm.controls.fullName;
  }

  get email() {
    return this.registerForm.controls.email;
  }

  get phone() {
    return this.registerForm.controls.phone;
  }

  get password() {
    return this.registerForm.controls.password;
  }

  get confirmPassword() {
    return this.registerForm.controls.confirmPassword;
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword =
      !this.showConfirmPassword;
  }

  onSubmit(): void {

    this.errorMessage = '';
    this.successMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    if (
      this.password.value !==
      this.confirmPassword.value
    ) {
      this.errorMessage =
        'Passwords do not match.';

      return;
    }

    const request: RegisterRequest = {

      fullName:
        this.fullName.value ?? '',

      email:
        this.email.value ?? '',

      phone:
        this.phone.value ?? '',

      password:
        this.password.value ?? '',

      confirmPassword:
        this.confirmPassword.value ?? ''

    };

    this.isLoading = true;

    this.authService
      .register(request)
      .subscribe({

        next: response => {

          this.isLoading = false;

          if (!response.success) {

            this.errorMessage =
              response.message ||
              'Registration failed.';

            return;
          }

          this.successMessage =
            response.message ||
            'Registration successful. Please verify your email.';

          this.registerForm.reset();

          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        },

        error: error => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to create your account. Please try again.';

        }

      });
  }
}