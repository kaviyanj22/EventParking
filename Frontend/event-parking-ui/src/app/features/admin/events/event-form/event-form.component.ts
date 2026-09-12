import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';

import {
  EventService
} from '../../../../core/services/event.service';

import {
  VenueService
} from '../../../../core/services/venue.service';

import {
  CategoryService
} from '../../../../core/services/category.service';

import {
  Event as EventModel,
  EventCreate,
  EventUpdate
} from '../../../../core/models/event.model';

import {
  Venue
} from '../../../../core/models/venue.model';

import {
  Category
} from '../../../../core/models/category.model';

import {
  environment
} from '../../../../../environments/environment';

@Component({
  selector: 'app-admin-event-form',

  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule
  ],

  templateUrl:
    './event-form.component.html',

  styleUrl:
    './event-form.component.css'
})
export class EventFormComponent
  implements OnChanges, OnDestroy {

  @Input()
  event: EventModel | null = null;

  @Output()
  saved =
    new EventEmitter<void>();

  @Output()
  cancelled =
    new EventEmitter<void>();

  venues: Venue[] = [];

  categories: Category[] = [];

  submitting = false;

  imageUploading = false;

  errorMessage = '';

  successMessage = '';

  imageErrorMessage = '';

  selectedEventImageFile:
    File | null = null;

  eventImagePreview:
    string | null = null;

  private objectPreviewUrl:
    string | null = null;

  todayDate =
    this.getTodayDate();

  eventForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private venueService: VenueService,
    private categoryService: CategoryService,
    private cdr: ChangeDetectorRef
  ) {

    this.eventForm =
      this.fb.group(
        {
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
            [
              Validators.required,
              this.notPastDateValidator()
            ]
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

          eventImageUrl: [
            null
          ],

          seatingLayoutImageUrl: [
            '',
            Validators.maxLength(500)
          ]
        },
        {
          validators: [
            this.timeRangeValidator()
          ]
        }
      );

    this.loadVenues();

    this.loadCategories();
  }

  // =========================================
  // CREATE / EDIT FORM
  // =========================================

  ngOnChanges(): void {

    this.todayDate =
      this.getTodayDate();

    this.clearLocalPreview();

    this.selectedEventImageFile =
      null;

    this.imageErrorMessage =
      '';

    if (this.event) {

      this.eventForm.patchValue({
        eventName:
          this.event.eventName,

        venueId:
          this.event.venueId,

        categoryId:
          this.event.categoryId,

        eventDate:
          this.event.eventDate
            .substring(0, 10),

        startTime:
          this.event.startTime,

        endTime:
          this.event.endTime,

        ticketPrice:
          this.event.ticketPrice,

        capacity:
          this.event.capacity,

        parkingFee:
          this.event.parkingFee,

        eventImageUrl:
          this.event.eventImageUrl ??
          null,

        seatingLayoutImageUrl:
          this.event
            .seatingLayoutImageUrl ??
          ''
      });

      this.eventImagePreview =
        this.buildImageUrl(
          this.event.eventImageUrl
        );

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
        eventImageUrl: null,
        seatingLayoutImageUrl: ''
      });

      this.eventImagePreview =
        null;
    }

    this.eventForm
      .updateValueAndValidity();

    this.cdr.detectChanges();
  }

  // =========================================
  // EVENT POSTER IMAGE SELECT
  // =========================================

  onEventImageSelected(
    event: globalThis.Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    const file =
      input.files?.[0];

    this.imageErrorMessage =
      '';

    if (!file) {
      return;
    }

    const allowedTypes = [
      'image/jpeg',
      'image/png',
      'image/webp'
    ];

    if (
      !allowedTypes.includes(
        file.type
      )
    ) {
      this.imageErrorMessage =
        'Only JPG, JPEG, PNG and WEBP images are allowed.';

      input.value = '';

      this.cdr.detectChanges();

      return;
    }

    const maxFileSize =
      5 * 1024 * 1024;

    if (
      file.size > maxFileSize
    ) {
      this.imageErrorMessage =
        'Image size cannot exceed 5 MB.';

      input.value = '';

      this.cdr.detectChanges();

      return;
    }

    this.clearLocalPreview();

    this.selectedEventImageFile =
      file;

    this.objectPreviewUrl =
      URL.createObjectURL(
        file
      );

    this.eventImagePreview =
      this.objectPreviewUrl;

    this.cdr.detectChanges();
  }

  // =========================================
  // REMOVE EVENT POSTER
  // =========================================

  removeEventImage(): void {

    this.clearLocalPreview();

    this.selectedEventImageFile =
      null;

    this.eventImagePreview =
      null;

    this.eventForm
      .get('eventImageUrl')
      ?.setValue(null);

    this.imageErrorMessage =
      '';

    this.cdr.detectChanges();
  }

  // =========================================
  // TODAY DATE
  // =========================================

  private getTodayDate(): string {

    const today =
      new Date();

    const year =
      today.getFullYear();

    const month =
      String(
        today.getMonth() + 1
      ).padStart(
        2,
        '0'
      );

    const day =
      String(
        today.getDate()
      ).padStart(
        2,
        '0'
      );

    return `${year}-${month}-${day}`;
  }

  // =========================================
  // DATE VALIDATION
  // =========================================

  private notPastDateValidator():
    ValidatorFn {

    return (
      control: AbstractControl
    ): ValidationErrors | null => {

      if (!control.value) {
        return null;
      }

      const selectedDate =
        new Date(
          `${control.value}T00:00:00`
        );

      const today =
        new Date();

      today.setHours(
        0,
        0,
        0,
        0
      );

      if (
        selectedDate < today
      ) {
        return {
          pastDate: true
        };
      }

      return null;
    };
  }

  // =========================================
  // TIME VALIDATION
  // =========================================

  private timeRangeValidator():
    ValidatorFn {

    return (
      control: AbstractControl
    ): ValidationErrors | null => {

      const startTime =
        control
          .get('startTime')
          ?.value;

      const endTime =
        control
          .get('endTime')
          ?.value;

      if (
        !startTime ||
        !endTime
      ) {
        return null;
      }

      if (
        endTime <= startTime
      ) {
        return {
          invalidTimeRange: true
        };
      }

      return null;
    };
  }

  // =========================================
  // LOAD VENUES
  // =========================================

  loadVenues(): void {

    this.venueService
      .getAll()
      .subscribe({

        next: (venues) => {

          this.venues =
            venues;

          this.cdr.detectChanges();
        },

        error: () => {

          this.errorMessage =
            'Unable to load venues.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // LOAD CATEGORIES
  // =========================================

  loadCategories(): void {

    this.categoryService
      .getAll()
      .subscribe({

        next: (categories) => {

          this.categories =
            categories;

          this.cdr.detectChanges();
        },

        error: () => {

          this.errorMessage =
            'Unable to load categories.';

          this.cdr.detectChanges();
        }

      });
  }

  // =========================================
  // SAVE EVENT
  // =========================================

  saveEvent(): void {

    if (
      this.eventForm.invalid
    ) {

      this.eventForm
        .markAllAsTouched();

      if (
        this.eventForm.hasError(
          'invalidTimeRange'
        )
      ) {
        this.errorMessage =
          'End time must be later than start time.';

      } else if (
        this.eventForm
          .get('eventDate')
          ?.hasError('pastDate')
      ) {
        this.errorMessage =
          'Event date cannot be earlier than today.';

      } else {
        this.errorMessage =
          'Please complete all required event details.';
      }

      this.cdr.detectChanges();

      return;
    }

    this.submitting =
      true;

    this.errorMessage =
      '';

    this.successMessage =
      '';

    if (
      this.selectedEventImageFile
    ) {

      this.imageUploading =
        true;

      this.eventService
        .uploadEventImage(
          this.selectedEventImageFile
        )
        .subscribe({

          next: (response) => {

            this.imageUploading =
              false;

            this.eventForm
              .get('eventImageUrl')
              ?.setValue(
                response.imageUrl
              );

            this.submitEvent();

            this.cdr.detectChanges();
          },

          error: (error) => {

            this.imageUploading =
              false;

            this.submitting =
              false;

            this.errorMessage =
              error?.error?.message ||
              'Unable to upload event image.';

            this.cdr.detectChanges();
          }

        });

      return;
    }

    this.submitEvent();
  }

  // =========================================
  // CREATE / UPDATE EVENT
  // =========================================

  private submitEvent(): void {

    const formValue =
      this.eventForm
        .getRawValue();

    const eventData:
      EventCreate = {

      eventName:
        formValue.eventName,

      venueId:
        Number(
          formValue.venueId
        ),

      categoryId:
        Number(
          formValue.categoryId
        ),

      eventDate:
        formValue.eventDate,

      startTime:
        formValue.startTime,

      endTime:
        formValue.endTime,

      ticketPrice:
        Number(
          formValue.ticketPrice
        ),

      capacity:
        Number(
          formValue.capacity
        ),

      parkingFee:
        Number(
          formValue.parkingFee
        ),

      eventImageUrl:
        formValue.eventImageUrl ||
        null,

      seatingLayoutImageUrl:
        formValue
          .seatingLayoutImageUrl ||
        null
    };

    if (this.event) {

      const updateData:
        EventUpdate = {
          ...eventData
        };

      this.eventService
        .update(
          this.event.eventId,
          updateData
        )
        .subscribe({

          next: () => {

            this.successMessage =
              'Event updated successfully.';

            this.submitting =
              false;

            this.selectedEventImageFile =
              null;

            this.saved.emit();

            this.cdr.detectChanges();
          },

          error: (error) => {

            this.errorMessage =
              error?.error?.message ||
              'Unable to update event.';

            this.submitting =
              false;

            this.cdr.detectChanges();
          }

        });

    } else {

      this.eventService
        .create(
          eventData
        )
        .subscribe({

          next: () => {

            this.successMessage =
              'Event created successfully.';

            this.submitting =
              false;

            this.selectedEventImageFile =
              null;

            this.saved.emit();

            this.cdr.detectChanges();
          },

          error: (error) => {

            this.errorMessage =
              error?.error?.message ||
              'Unable to create event.';

            this.submitting =
              false;

            this.cdr.detectChanges();
          }

        });
    }
  }

  // =========================================
  // BUILD BACKEND IMAGE URL
  // =========================================

  private buildImageUrl(
    imageUrl:
      string | null | undefined
  ): string | null {

    if (!imageUrl) {
      return null;
    }

    if (
      imageUrl.startsWith(
        'http://'
      ) ||
      imageUrl.startsWith(
        'https://'
      )
    ) {
      return imageUrl;
    }

    const backendBaseUrl =
      environment.apiUrl
        .replace(
          /\/api\/?$/,
          ''
        );

    return `${backendBaseUrl}${imageUrl}`;
  }

  // =========================================
  // CLEAR PREVIEW
  // =========================================

  private clearLocalPreview(): void {

    if (
      this.objectPreviewUrl
    ) {
      URL.revokeObjectURL(
        this.objectPreviewUrl
      );

      this.objectPreviewUrl =
        null;
    }
  }

  // =========================================
  // CANCEL
  // =========================================

  cancel(): void {

    this.clearLocalPreview();

    this.cancelled.emit();
  }

  // =========================================
  // DESTROY
  // =========================================

  ngOnDestroy(): void {

    this.clearLocalPreview();
  }
}