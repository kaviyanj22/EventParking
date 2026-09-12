import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output
} from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

import {
  EventService
} from '../../../core/services/event.service';

import {
  Event
} from '../../../core/models/event.model';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.css'
})
export class EventDetailsComponent implements OnChanges {

  @Input()
  eventId!: number;

  @Output()
  bookNow =
    new EventEmitter<number>();

  event: Event | null = null;

  loading = false;

  errorMessage = '';

constructor(
  private eventService: EventService,
  private cdr: ChangeDetectorRef,
  private authService: AuthService
) {}

get isAdmin(): boolean {
  return this.authService.isAdmin();
}
  ngOnChanges(): void {

    if (this.eventId) {
      this.loadEvent();
    }
  }

  loadEvent(): void {

    this.loading = true;

    this.errorMessage = '';

    this.eventService
      .getById(this.eventId)
      .subscribe({

        next: (event) => {

          this.event = event;

          this.loading = false;

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.event = null;

          this.loading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to load event details.';

          this.cdr.markForCheck();
        }

      });
  }

  onBookNow(): void {

    if (this.event) {

      this.bookNow.emit(
        this.event.eventId
      );
    }
  }
}