import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Event } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-card.component.html',
  styleUrl: './event-card.component.css'
})
export class EventCardComponent {

  @Input({ required: true })
  event!: Event;

  @Output()
  viewDetails = new EventEmitter<number>();

  onViewDetails(): void {
    this.viewDetails.emit(this.event.eventId);
  }
}