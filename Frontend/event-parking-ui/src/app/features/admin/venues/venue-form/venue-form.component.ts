import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { VenueService } from '../../../../core/services/venue.service';
import {
  Venue,
  VenueCreate,
  VenueUpdate
} from '../../../../core/models/venue.model';

@Component({
  selector: 'app-venue-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './venue-form.component.html',
  styleUrl: './venue-form.component.css'
})
export class VenueFormComponent implements OnChanges {

  @Input()
  venue: Venue | null = null;

  @Output()
  saved = new EventEmitter<void>();

  @Output()
  cancelled = new EventEmitter<void>();

  formData: VenueCreate = {
    venueName: '',
    address: '',
    capacity: 1
  };

  submitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private venueService: VenueService
  ) {}

  ngOnChanges(): void {
    if (this.venue) {
      this.formData = {
        venueName: this.venue.venueName,
        address: this.venue.address,
        capacity: this.venue.capacity
      };
    } else {
      this.resetForm();
    }
  }

  saveVenue(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.venue) {
      const updateData: VenueUpdate = {
        ...this.formData
      };

      this.venueService
        .update(this.venue.venueId, updateData)
        .subscribe({
          next: () => {
            this.successMessage =
              'Venue updated successfully.';
            this.submitting = false;
            this.saved.emit();
          },
          error: (error) => {
            this.errorMessage =
              error?.error?.message ||
              'Unable to update venue.';
            this.submitting = false;
          }
        });

    } else {

      this.venueService
        .create(this.formData)
        .subscribe({
          next: () => {
            this.successMessage =
              'Venue created successfully.';
            this.submitting = false;
            this.resetForm();
            this.saved.emit();
          },
          error: (error) => {
            this.errorMessage =
              error?.error?.message ||
              'Unable to create venue.';
            this.submitting = false;
          }
        });
    }
  }

  cancel(): void {
    this.resetForm();
    this.cancelled.emit();
  }

  resetForm(): void {
    this.formData = {
      venueName: '',
      address: '',
      capacity: 1
    };

    this.errorMessage = '';
  }
}