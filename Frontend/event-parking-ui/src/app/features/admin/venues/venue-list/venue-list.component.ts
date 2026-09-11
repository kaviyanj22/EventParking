import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VenueService } from '../../../../core/services/venue.service';
import { Venue } from '../../../../core/models/venue.model';
import { VenueFormComponent } from '../venue-form/venue-form.component';
@Component({
  selector: 'app-venue-list',
  standalone: true,
  imports: [
  CommonModule,
  VenueFormComponent
],
  templateUrl: './venue-list.component.html',
  styleUrl: './venue-list.component.css'
})
export class VenueListComponent implements OnInit {

  venues: Venue[] = [];

  loading = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;
  selectedVenue: Venue | null = null;

  constructor(
    private venueService: VenueService
  ) {}

  ngOnInit(): void {
    this.loadVenues();
  }

  loadVenues(): void {
    this.loading = true;
    this.errorMessage = '';

    this.venueService.getAll().subscribe({
      next: (venues) => {
        this.venues = venues;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load venues.';
        this.loading = false;
      }
    });
  }
  
  addVenue(): void {
  this.selectedVenue = null;
  this.showForm = true;
}

 onVenueSaved(): void {
  this.showForm = false;
  this.selectedVenue = null;
  this.loadVenues();
}

onVenueCancelled(): void {
  this.showForm = false;
  this.selectedVenue = null;
  
}
  editVenue(venueId: number): void {
  const venue = this.venues.find(
    v => v.venueId === venueId
  );

  

  if (venue) {
    this.selectedVenue = venue;
    this.showForm = true;
  }
}

  deleteVenue(venueId: number): void {

    const confirmed =
      window.confirm(
        'Are you sure you want to delete this venue?'
      );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.venueService.delete(venueId).subscribe({
      next: () => {
        this.successMessage =
          'Venue deleted successfully.';

        this.loadVenues();
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message ||
          'Unable to delete venue.';
      }
    });
  }
}