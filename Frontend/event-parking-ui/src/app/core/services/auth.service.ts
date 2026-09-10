import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { AuthResponse } from '../models/auth-response.model';
import { ForgotPasswordRequest } from '../models/forgot-password.model';
import { ResetPasswordRequest } from '../models/reset-password.model';
import { ResendVerificationRequest } from '../models/resend-verification.model';

export interface ServiceResult<T> {
  success: boolean;
  message: string;
  data: T | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl = 'https://localhost:7168/api/auth';;

  private readonly tokenKey = 'auth_token';
  private readonly userKey = 'auth_user';

  private currentUserSubject =
    new BehaviorSubject<AuthResponse | null>(
      this.getStoredUser()
    );

  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  // =========================
  // REGISTER
  // =========================

  register(
    request: RegisterRequest
  ): Observable<ServiceResult<object>> {

    return this.http.post<ServiceResult<object>>(
      `${this.apiUrl}/register`,
      request
    );
  }

  // =========================
  // LOGIN
  // =========================

  login(
    request: LoginRequest
  ): Observable<ServiceResult<AuthResponse>> {

    return this.http
      .post<ServiceResult<AuthResponse>>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {

          if (
            response.success &&
            response.data
          ) {
            this.saveLoginData(response.data);
          }

        })
      );
  }

  // =========================
  // VERIFY EMAIL
  // =========================

  verifyEmail(
    token: string
  ): Observable<ServiceResult<object>> {

    return this.http.get<ServiceResult<object>>(
      `${this.apiUrl}/verify-email`,
      {
        params: {
          token: token
        }
      }
    );
  }

  // =========================
  // RESEND VERIFICATION
  // =========================

  resendVerification(
    request: ResendVerificationRequest
  ): Observable<ServiceResult<object>> {

    return this.http.post<ServiceResult<object>>(
      `${this.apiUrl}/resend-verification`,
      request
    );
  }

  // =========================
  // FORGOT PASSWORD
  // =========================

  forgotPassword(
    request: ForgotPasswordRequest
  ): Observable<ServiceResult<object>> {

    return this.http.post<ServiceResult<object>>(
      `${this.apiUrl}/forgot-password`,
      request
    );
  }

  // =========================
  // RESET PASSWORD
  // =========================

  resetPassword(
    request: ResetPasswordRequest
  ): Observable<ServiceResult<object>> {

    return this.http.post<ServiceResult<object>>(
      `${this.apiUrl}/reset-password`,
      request
    );
  }

  // =========================
  // SAVE LOGIN DATA
  // =========================

  private saveLoginData(
    authData: AuthResponse
  ): void {

    localStorage.setItem(
      this.tokenKey,
      authData.token
    );

    localStorage.setItem(
      this.userKey,
      JSON.stringify(authData)
    );

    this.currentUserSubject.next(authData);
  }

  // =========================
  // GET TOKEN
  // =========================

  getToken(): string | null {

    return localStorage.getItem(
      this.tokenKey
    );
  }

  // =========================
  // GET CURRENT USER
  // =========================

  getCurrentUser(): AuthResponse | null {

    return this.currentUserSubject.value;
  }

  // =========================
  // GET STORED USER
  // =========================

  private getStoredUser(): AuthResponse | null {

    const user =
      localStorage.getItem(this.userKey);

    if (!user) {
      return null;
    }

    try {
      return JSON.parse(user) as AuthResponse;
    }
    catch {
      localStorage.removeItem(this.userKey);
      return null;
    }
  }

  // =========================
  // CHECK LOGIN
  // =========================

  isLoggedIn(): boolean {

    const token = this.getToken();

    if (!token) {
      return false;
    }

    const user = this.getCurrentUser();

    if (!user) {
      return false;
    }

    const expiry =
      new Date(user.expiresAt).getTime();

    if (Date.now() >= expiry) {
      this.logout();
      return false;
    }

    return true;
  }

  // =========================
  // CHECK ADMIN
  // =========================

  isAdmin(): boolean {

    const user = this.getCurrentUser();

    return user?.role
      ?.toLowerCase() === 'admin';
  }

  // =========================
  // LOGOUT
  // =========================

  logout(): void {

    localStorage.removeItem(
      this.tokenKey
    );

    localStorage.removeItem(
      this.userKey
    );

    this.currentUserSubject.next(null);
  }
}