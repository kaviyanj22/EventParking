export interface ParkingSlot {
  parkingSlotId: number;
  eventId: number;
  slotNumber: string;
  zone: string | null;
  fee: number;
  status: string;
}
export interface ParkingSlotCreate {
  slotNumber: string;
  zone: string | null;
  fee: number;
}
export interface ParkingSlotUpdate {
  slotNumber: string;
  zone: string | null;
  fee: number;
  status: string;
}
export interface ParkingLayoutCreate {
  parkingSlots: ParkingSlotCreate[];
}