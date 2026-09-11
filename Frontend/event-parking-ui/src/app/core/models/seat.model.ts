export interface Seat {
  seatId: number;
  eventId: number;
  seatSectionId: number | null;
  sectionName: string | null;
  seatNumber: string;
  rowName: string | null;
  columnNumber: number | null;
  positionX: number | null;
  positionY: number | null;
  seatType: string | null;
  price: number | null;
  status: string;
}

export interface SeatCreate {
  seatSectionId: number | null;
  seatNumber: string;
  rowName: string | null;
  columnNumber: number | null;
  positionX: number | null;
  positionY: number | null;
  seatType: string | null;
  price: number | null;
}
export interface SeatUpdate {
  seatSectionId: number | null;
  seatNumber: string;
  rowName: string | null;
  columnNumber: number | null;
  positionX: number | null;
  positionY: number | null;
  seatType: string | null;
  price: number | null;
  status: string;
}
export interface SeatMapCreate {
  seats: SeatCreate[];
}