export interface Event {
  eventId: number;
  eventName: string;

  venueId: number;
  venueName: string;

  categoryId: number;
  categoryName: string;

  eventDate: string;
  startTime: string;
  endTime: string;

  ticketPrice: number;
  capacity: number;
  parkingFee: number;

  // Customer-facing event poster/banner
  eventImageUrl: string | null;

  // Seating layout image
  seatingLayoutImageUrl: string | null;

  createdAt: string;
  updatedAt: string | null;
}

export interface EventCreate {
  eventName: string;

  venueId: number;
  categoryId: number;

  eventDate: string;
  startTime: string;
  endTime: string;

  ticketPrice: number;
  capacity: number;
  parkingFee: number;

  // Customer-facing event poster/banner
  eventImageUrl: string | null;

  // Seating layout image
  seatingLayoutImageUrl: string | null;
}

export interface EventUpdate {
  eventName: string;

  venueId: number;
  categoryId: number;

  eventDate: string;
  startTime: string;
  endTime: string;

  ticketPrice: number;
  capacity: number;
  parkingFee: number;

  // Customer-facing event poster/banner
  eventImageUrl: string | null;

  // Seating layout image
  seatingLayoutImageUrl: string | null;
}

export interface EventFilter {
  name?: string;
  date?: string;
  venueId?: number;
  categoryId?: number;
}