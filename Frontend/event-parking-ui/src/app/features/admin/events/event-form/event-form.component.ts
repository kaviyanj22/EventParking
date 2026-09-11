import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { EventService } from '../../../../core/services/event.service';
import { VenueService } from '../../../../core/services/venue.service';
import { CategoryService } from '../../../../core/services/category.service';

import {
  Event,
  EventCreate,
  EventUpdate
} from '../../../../core/models/event.model';

import { Venue } from '../../../../core/models/venue.model';
import { Category } from '../../../../core/models/category.model';

@Component({
  selector: 'app-admin-event-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './event-form.component.html',
  styleUrl: './event-form.component.css'
})
export class EventFormComponent implements OnChanges {

  @Input()
  event: Event | null = null;

  @Output()
  saved = new EventEmitter<void>();

  @Output()
  cancelled = new EventEmitter<void>();

  venues: Venue[] = [];
  categories: Category[] = [];

  submitting = false;
  errorMessage = '';
  successMessage = '';

  eventForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private venueService: VenueService,
    private categoryService: CategoryService
  ) {

    this.eventForm = this.fb.group({

      eventName: [
        '',
        [
          Validators.required,
          Validators.maxLength(150)
        ]
      ],

      venueId: [
        0,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],

      categoryId: [
        0,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],

      eventDate: [
        '',
        Validators.required
      ],

      startTime: [
        '',
        Validators.required
      ],

      endTime: [
        '',
        Validators.required
      ],

      ticketPrice: [
        0,
        [
          Validators.required,
          Validators.min(0)
        ]
      ],

      capacity: [
        1,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],

      parkingFee: [
        0,
        [
          Validators.required,
          Validators.min(0)
        ]
      ],

      seatingLayoutImageUrl: [
        '',
        Validators.maxLength(500)
      ]

    });

    this.loadVenues();
    this.loadCategories();
  }

  ngOnChanges(): void {

    if (this.event) {

      this.eventForm.patchValue({
        eventName: this.event.eventName,
        venueId: this.event.venueId,
        categoryId: this.event.categoryId,
        eventDate: this.event.eventDate.substring(0, 10),
        startTime: this.event.startTime,
        endTime: this.event.endTime,
        ticketPrice: this.event.ticketPrice,
        capacity: this.event.capacity,
        parkingFee: this.event.parkingFee,
        seatingLayoutImageUrl:
          this.event.seatingLayoutImageUrl ?? ''
      });

    } else {

      this.eventForm.reset({
        eventName: '',
        venueId: 0,
        categoryId: 0,
        eventDate: '',
        startTime: '',
        endTime: '',
        ticketPrice: 0,
        capacity: 1,
        parkingFee: 0,
        seatingLayoutImageUrl: ''
      });

    }
  }

  loadVenues(): void {

    this.venueService.getAll().subscribe({

      next: (venues) => {
        this.venues = venues;
      },

      error: () => {
        this.errorMessage =
          'Unable to load venues.';
      }

    });
  }

  loadCategories(): void {

    this.categoryService.getAll().subscribe({

      next: (categories) => {
        this.categories = categories;
      },

      error: () => {
        this.errorMessage =
          'Unable to load categories.';
      }

    });
  }
  saveEvent(): void {

  if (this.eventForm.invalid) {
    this.eventForm.markAllAsTouched();
    return;
  }

  this.submitting = true;
  this.errorMessage = '';
  this.successMessage = '';

  const formValue = this.eventForm.getRawValue();

  const eventData: EventCreate = {
    eventName: formValue.eventName,
    venueId: Number(formValue.venueId),
    categoryId: Number(formValue.categoryId),
    eventDate: formValue.eventDate,
    startTime: formValue.startTime,
    endTime: formValue.endTime,
    ticketPrice: Number(formValue.ticketPrice),
    capacity: Number(formValue.capacity),
    parkingFee: Number(formValue.parkingFee),
    seatingLayoutImageUrl:
      formValue.seatingLayoutImageUrl || null
  };

  if (this.event) {

    const updateData: EventUpdate = {
      ...eventData
    };

    this.eventService
      .update(this.event.eventId, updateData)
      .subscribe({
        next: () => {
          this.successMessage =
            'Event updated successfully.';

          this.submitting = false;
          this.saved.emit();
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ||
            'Unable to update event.';

          this.submitting = false;
        }
      });

  } else {

    this.eventService
      .create(eventData)
      .subscribe({
        next: () => {
          this.successMessage =
            'Event created successfully.';

          this.submitting = false;
          this.saved.emit();
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ||
            'Unable to create event.';

          this.submitting = false;
        }
      });
  }
}

cancel(): void {
  this.cancelled.emit();
}
}