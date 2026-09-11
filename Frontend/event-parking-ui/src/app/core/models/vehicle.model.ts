export interface Vehicle {
  vehicleId: number;
  customerId: number;
  vehicleType: string;
  vehicleNumber: string;
  make?: string | null;
  model?: string | null;
  color?: string | null;
  createdAt: string;
}