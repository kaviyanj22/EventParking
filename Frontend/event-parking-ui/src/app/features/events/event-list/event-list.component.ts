import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { EventService } from '../../../core/services/event.service';
import { VenueService } from '../../../core/services/venue.service';
import { CategoryService } from '../../../core/services/category.service';
import { EventDetailsComponent } from '../event-details/event-details.component';
import {
  Event,
  EventFilter
} from '../../../core/models/event.model';

import { Venue } from '../../../core/models/venue.model';
import { Category } from '../../../core/models/category.model';
import { EventCardComponent } from '../event-card/event-card.component';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,  
     EventCardComponent,
     EventDetailsComponent
  ],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.css'
})
export class EventListComponent implements OnInit {

  events: Event[] = [];
  venues: Venue[] = [];
  categories: Category[] = [];

  selectedEventId: number | null = null;

  loading = false;
  errorMessage = '';

  filter: EventFilter = {
    name: '',
    date: undefined,
    venueId: undefined,
    categoryId: undefined
  };

  constructor(
    private eventService: EventService,
    private venueService: VenueService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.loadEvents();
    this.loadVenues();
    this.loadCategories();
  }

  loadEvents(): void {
    this.loading = true;
    this.errorMessage = '';

    this.eventService.getAll(this.filter).subscribe({
      next: (events) => {
        this.events = events;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load events.';
        this.loading = false;
      }
    });
  }

  loadVenues(): void {
    this.venueService.getAll().subscribe({
      next: (venues) => {
        this.venues = venues;
      },
      error: () => {
        this.errorMessage = 'Unable to load venues.';
      }
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: () => {
        this.errorMessage = 'Unable to load categories.';
      }
    });
  }

  search(): void {
    this.loadEvents();
  }

  clearFilters(): void {
    this.filter = {
      name: '',
      date: undefined,
      venueId: undefined,
      categoryId: undefined
    };

    this.loadEvents();
  }
  onViewDetails(eventId: number): void {
  this.selectedEventId = eventId;
}

  closeEventDetails(): void {
  this.selectedEventId = null;
}
onBookNow(eventId: number): void {
  console.log('Book Now Event ID:', eventId);
}
}