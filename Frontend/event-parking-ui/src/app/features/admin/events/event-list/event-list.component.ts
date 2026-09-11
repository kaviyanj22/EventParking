import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EventService } from '../../../../core/services/event.service';
import { Event } from '../../../../core/models/event.model';
import { EventFormComponent } from '../event-form/event-form.component';

@Component({
  selector: 'app-admin-event-list',
  standalone: true,
  imports: [
  CommonModule,
  EventFormComponent,
 
],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.css'
})
export class EventListComponent implements OnInit {

  events: Event[] = [];

  loading = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;
selectedEvent: Event | null = null;

  constructor(
    private eventService: EventService
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loading = true;
    this.errorMessage = '';

    this.eventService.getAll().subscribe({
      next: (events) => {
        this.events = events;
        this.loading = false;
      },
      error: () => {
        this.errorMessage =
          'Unable to load events.';
        this.loading = false;
      }
    });
  }

  addEvent(): void {
  this.selectedEvent = null;
  this.showForm = true;
}

onEventSaved(): void {
  this.showForm = false;
  this.selectedEvent = null;
  this.loadEvents();
}

onEventCancelled(): void {
  this.showForm = false;
  this.selectedEvent = null;
}

  editEvent(eventId: number): void {
  const event = this.events.find(
    e => e.eventId === eventId
  );

  if (event) {
    this.selectedEvent = event;
    this.showForm = true;
  }
}

  deleteEvent(eventId: number): void {

    const confirmed = window.confirm(
      'Are you sure you want to delete this event?'
    );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.delete(eventId).subscribe({
      next: () => {
        this.successMessage =
          'Event deleted successfully.';

        this.loadEvents();
      },
      error: (error) => {
        this.errorMessage =
          error?.error?.message ||
          'Unable to delete event.';
      }
    });
  }
}