export interface Customer {
  customerId: number;
  fullName: string;
  email: string;
  phone: string;
  role: string;
  status: string;
  emailVerified: boolean;
  createdAt: string;
  updatedAt?: string | null;
}