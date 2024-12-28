import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CitiesService {
  cities: City[] = [];

  constructor(private httpClient: HttpClient) {}

  public getCities(): Observable<City[]> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer myToken"); // Dummy auth header
    return this.httpClient.get<City[]>('http://localhost:5104/api/v1/Cities', { headers: headers });
  }
}
