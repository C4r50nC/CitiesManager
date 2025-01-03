import { Routes } from '@angular/router';
import { CitiesComponent } from './cities/cities.component';
import { RegisterComponent } from './register/register.component';

export const routes: Routes = [
  { path: 'cities', component: CitiesComponent },
  { path: 'register', component: RegisterComponent },
];
