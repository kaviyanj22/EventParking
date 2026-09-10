import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit
} from '@angular/core';
import {
  ActivatedRoute,
  RouterLink
} from '@angular/router';

import {
  AuthService
} from '../../../core/services/auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.css'
})
export class VerifyEmail implements OnInit {

  isLoading = true;

  isSuccess = false;

  message = '';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit(): void {

    const token =
      this.route.snapshot.queryParamMap
        .get('token');

    if (!token) {

      this.isLoading = false;

      this.isSuccess = false;

      this.message =
        'Verification token is missing.';

      return;
    }

    this.authService
      .verifyEmail(token)
      .subscribe({

        next: response => {

          this.isLoading = false;

          this.isSuccess =
            response.success;

          this.message =
            response.message ||
            (
              response.success
                ? 'Email verified successfully.'
                : 'Email verification failed.'
            );
        },

        error: error => {

          this.isLoading = false;

          this.isSuccess = false;

          this.message =
            error?.error?.message ||
            'Unable to verify your email. The verification link may be invalid or expired.';
        }

      });
  }
}