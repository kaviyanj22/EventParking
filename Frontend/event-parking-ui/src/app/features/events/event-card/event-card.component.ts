import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  Event
} from '../../../core/models/event.model';

import {
  environment
} from '../../../../environments/environment';

@Component({
  selector: 'app-event-card',
  standalone: true,

  imports: [
    CommonModule
  ],

  templateUrl:
    './event-card.component.html',

  styleUrl:
    './event-card.component.css'
})
export class EventCardComponent {

  @Input({ required: true })
  event!: Event;

  @Output()
  viewDetails =
    new EventEmitter<number>();

  imageLoadFailed = false;

  // =========================================
  // EVENT POSTER URL
  // =========================================

  get eventImageUrl():
    string | null {

    const imageUrl =
      this.event?.eventImageUrl;

    if (!imageUrl) {
      return null;
    }

    if (
      imageUrl.startsWith('http://') ||
      imageUrl.startsWith('https://')
    ) {
      return imageUrl;
    }

    const backendBaseUrl =
      environment.apiUrl.replace(
        /\/api\/?$/,
        ''
      );

    return `${backendBaseUrl}${imageUrl}`;
  }

  // =========================================
  // IMAGE ERROR
  // =========================================

  onImageError(): void {
    this.imageLoadFailed = true;
  }

  // =========================================
  // VIEW DETAILS
  // =========================================

  onViewDetails(): void {
    this.viewDetails.emit(
      this.event.eventId
    );
  }
}