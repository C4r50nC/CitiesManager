import { Component } from '@angular/core';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';
import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DisableControlDirective } from '../directives/disable-control.directive';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-cities',
  imports: [ReactiveFormsModule, CommonModule, DisableControlDirective],
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.css',
})
export class CitiesComponent {
  cities: City[] = [];

  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;

  putCityForm: FormGroup;
  editedCityId: string | null = null;

  constructor(
    private citiesService: CitiesService,
    private accountsService: AccountsService
  ) {
    this.postCityForm = new FormGroup({
      cityName: new FormControl(null, [Validators.required]),
    });

    this.putCityForm = new FormGroup({
      cities: new FormArray([]),
    });
  }

  get putCityFormArray(): FormArray {
    return this.putCityForm.get('cities') as FormArray;
  }

  loadCities() {
    this.citiesService.getCities().subscribe({
      next: (response: City[]) => {
        this.cities = response;

        this.cities.forEach((city) => {
          this.putCityFormArray.push(
            new FormGroup({
              cityId: new FormControl(city.cityId, [Validators.required]),
              cityName: new FormControl(
                { value: city.cityName, disabled: true },
                [Validators.required]
              ),
            })
          );
        });
      },
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
        this.putCityFormArray.push(
          new FormGroup({
            cityId: new FormControl(response.cityId, [Validators.required]),
            cityName: new FormControl(
              { value: response.cityName, disabled: true },
              [Validators.required]
            ),
          })
        );
        this.cities.push(new City(response.cityId, response.cityName));
        this.postCityForm.reset();
        this.isPostCityFormSubmitted = false; // Reset error message status
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }

  editClicked(city: City): void {
    this.editedCityId = city.cityId;
  }

  updateClicked(index: number): void {
    this.citiesService
      .putCity(this.putCityFormArray.controls[index].value)
      .subscribe({
        next: (response: string) => {
          console.log(response);
          this.putCityFormArray.controls[index].reset(
            this.putCityFormArray.controls[index].value
          );
          this.editedCityId = null;
        },
        error: (error: any) => console.error(error),
        complete: () => {},
      });
  }

  deleteClicked(city: City, index: number): void {
    if (!confirm(`Are you sure to delete this city: ${city.cityName}`)) {
      return;
    }

    this.citiesService.deleteCity(city.cityId).subscribe({
      next: (response: string) => {
        console.log(response);
        this.putCityFormArray.removeAt(index);
        this.cities.splice(index, 1);
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }

  // Can be transformed to an automated process without button clicks
  refreshClicked(): void {
    this.accountsService.postGenerateNewToken().subscribe({
      next: (response: any) => {
        console.log(response);
        localStorage['token'] = response.token;
        localStorage['refreshToken'] = response.refreshToken;

        this.loadCities();
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }
}
