import { Component } from '@angular/core';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-cities',
  imports: [ReactiveFormsModule],
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.css',
})
export class CitiesComponent {
  cities: City[] = [];
  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;

  constructor(private citiesService: CitiesService) {
    this.postCityForm = new FormGroup({
      cityName: new FormControl(null, [Validators.required]),
    });
  }

  loadCities() {
    this.citiesService.getCities().subscribe({
      next: (response: City[]) => (this.cities = response),
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }

  ngOnInit() {
    this.loadCities();
  }

  get postCity_CityNameControl(): any {
    return this.postCityForm.controls['cityName'];
  }

  public postCitySubmitted() {
    this.isPostCityFormSubmitted = true;

    console.log(this.postCityForm.value);

    this.citiesService.postCity(this.postCityForm.value).subscribe({
      next: (response: City) => {
        console.log(response);
        this.cities.push(new City(response.cityId, response.cityName));
        this.postCityForm.reset();
        this.isPostCityFormSubmitted = false; // Reset error message status
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }
}
