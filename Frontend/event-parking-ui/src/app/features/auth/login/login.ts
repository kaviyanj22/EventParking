import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
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
  LoginRequest
} from '../../../core/models/login-request.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  isLoading = false;

  errorMessage = '';

  showPassword = false;

  loginForm;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {

    this.loginForm = this.formBuilder.group({
      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],

      password: [
        '',
        [
          Validators.required
        ]
      ]
    });
  }

  get email() {
    return this.loginForm.controls.email;
  }

  get password() {
    return this.loginForm.controls.password;
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {

    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const request: LoginRequest = {
      email:
        this.loginForm.value.email ?? '',

      password:
        this.loginForm.value.password ?? ''
    };

    this.isLoading = true;

    this.authService
      .login(request)
      .subscribe({

        next: response => {

          this.isLoading = false;

          if (
            !response.success ||
            !response.data
          ) {
            this.errorMessage =
              response.message ||
              'Login failed. Please try again.';

            return;
          }

          // Admin → Admin Dashboard
          if (
            response.data.role
              .toLowerCase() === 'admin'
          ) {
            this.router.navigate(['/admin']);
            return;
          }

          // Customer originally tried to access
          // a protected page
          const returnUrl =
            this.route.snapshot.queryParamMap
              .get('returnUrl');

          if (returnUrl) {
            this.router.navigateByUrl(returnUrl);
            return;
          }

          // Normal customer login
          this.router.navigate(['/events']);
        },

        error: error => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to login. Please check your email and password.';
        }

      });
  }
}