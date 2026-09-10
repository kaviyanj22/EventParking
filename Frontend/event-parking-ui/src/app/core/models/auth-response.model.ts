export interface AuthResponse {
  token: string;
  expiresAt: string;
  customerId: number;
  fullName: string;
  email: string;
  role: string;
}
