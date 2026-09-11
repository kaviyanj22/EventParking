import { Routes } from '@angular/router';
import { ParkingMapComponent } from './features/parking/parking-map/parking-map';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'parking/1',
    pathMatch: 'full'
  },
  {
    path: 'parking/:id',
    component: ParkingMapComponent
  }
];