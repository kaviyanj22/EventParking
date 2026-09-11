import { Output,Component, Input, OnChanges,EventEmitter, } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EventService } from '../../../core/services/event.service';
import { Event } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.css'
})
export class EventDetailsComponent implements OnChanges {

  @Input()
  eventId!: number;
  event: Event | null = null;
  loading = false;
  errorMessage = '';
  
  @Output()
bookNow = new EventEmitter<number>();

  constructor(
    private eventService: EventService
  ) {}

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
        },
        error: () => {
          this.event = null;
          this.errorMessage =
            'Unable to load event details.';
          this.loading = false;
        }
      });
  }

  onBookNow(): void {
  if (this.event) {
    this.bookNow.emit(this.event.eventId);
  }
}
}