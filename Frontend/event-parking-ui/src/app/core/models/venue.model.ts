export interface Venue {
  venueId: number;
  venueName: string;
  address: string;
  capacity: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface VenueCreate {
  venueName: string;
  address: string;
  capacity: number;
}

export interface VenueUpdate {
  venueName: string;
  address: string;
  capacity: number;
}